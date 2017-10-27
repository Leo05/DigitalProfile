using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using Newtonsoft.Json;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Ionic.Zip;
using System.IO;
using System.Threading.Tasks;

namespace core
{
    class digitalprofile
    {

        string mongoHost = System.Configuration.ConfigurationManager.AppSettings["MongoHost"];
        string mongoPort = System.Configuration.ConfigurationManager.AppSettings["MongoPort"];
        string connectionString1 = string.Empty;
        MongoClient _client;
        MongoServer _server;
        MongoDatabase _db;

        public digitalprofile()
        {
            connectionString1 = "mongodb://" + mongoHost + ":" + mongoPort;
            _client = new MongoClient(connectionString1);
            _server = _client.GetServer();
            _db = _server.GetDatabase("digital");
        }
        public class Documents
        {
            public ObjectId refkey { get; set; }
            public string key { get; set; }
        }
        public class DocTypes
        {
            public string docname { get; set; }
            public string doccode { get; set; }
        }
        public class DocsValidator
        {
            public DocsValidator(string _docname, string _dockey, bool _isfound)
            {
                docname = _docname;
                dockey = _dockey;
                isfound = _isfound;
            }
            public string docname { get; set; }
            public string dockey { get; set; }
            public bool isfound { get; set; }
        }
        public string GetDocumentInfo(string CollectionName, string searchKey, string id_docfilename, string _ref)
        {
            string newFileName = string.Empty;
            BsonValue myId_pdf;
            MongoCollection<BsonDocument> _collection = _db.GetCollection<BsonDocument>(CollectionName);
            var query = Query.EQ("idfilename", id_docfilename);
            var query_id = Query.EQ("idfilename", ObjectId.Parse(id_docfilename));
            var cursor = _collection.Find(query_id);
            string filename = string.Empty;
            string jsondoc = string.Empty;
            foreach (var sdoc in cursor)
            {
                myId_pdf = (BsonValue)sdoc["idfilename"];
                newProfile mydoc = new newProfile();
                mydoc.Id = sdoc["_id"].ToString();
                mydoc.key = getArrayValuesToString(sdoc["key"].AsBsonArray);
                mydoc.key2 = getArrayValuesToString(sdoc["key2"].AsBsonArray);
                mydoc.content_type = sdoc["content_type"].AsString;
                mydoc.idfilename = sdoc["idfilename"].ToString();
                mydoc.filename = sdoc["filename"].ToString();
                mydoc.lenght = sdoc["lenght"].ToDouble();
                mydoc.uploaddate = sdoc["uploaddate"].ToString();
                mydoc.doctype = sdoc["doctype"].ToString();
                mydoc.doctypename = GetCollDataFieldValue("DocTypes", "code", mydoc.doctype, "name");
                if (mydoc.doctypename == "")
                    mydoc.doctypename = "NO DEFINIDO";

                mydoc.docdate = sdoc["docdate"].ToString();
                mydoc.expdate = sdoc.GetValue("dateto", "N/A").ToString();
                mydoc.tags = getArrayValuesToString(sdoc["tags"].AsBsonArray);
                mydoc.year = sdoc["year"].ToInt32();
                mydoc.month = sdoc["month"].ToInt32();
                mydoc.day = sdoc["day"].ToInt32();
                mydoc.source = sdoc["source"].ToString();
                mydoc.comments = sdoc["comments"].ToString();
                mydoc.user = sdoc.GetValue("user", "N/A").ToString();
                mydoc.fullfilename = System.IO.Path.GetFileName(QueryFileId(myId_pdf, sdoc["content_type"].AsString, _ref));
                
                //mydoc.controlid = sdoc["controlid"].ToString();
                jsondoc = JsonConvert.SerializeObject(mydoc, Formatting.None);
            }
            return jsondoc;
        }
        public string GetDocumentInfoZip(string CollectionName, string searchKey, string id_docfilename, string fullfilename, string _ref)
        {
            string newFileName = string.Empty;
            BsonValue myId_pdf;
            MongoCollection<BsonDocument> _collection = _db.GetCollection<BsonDocument>(CollectionName);
            var query = Query.EQ("idfilename", id_docfilename);
            var query_id = Query.EQ("idfilename", ObjectId.Parse(id_docfilename));
            var cursor = _collection.Find(query_id);
            string filename = string.Empty;
            foreach (var sdoc in cursor)
            {
                myId_pdf = (BsonValue)sdoc["idfilename"];
                filename = QueryZipFileId(myId_pdf, sdoc["content_type"].AsString, fullfilename, _ref);
            }
            return System.IO.Path.GetFileName(filename);
        }
        public string GetSelectedintoZip(string CollectionName, string searchKey, string id_docfilename, string fullfilenames, string _ref, string mailto, string tipodoc)
        {
            System.Web.HttpContext c = System.Web.HttpContext.Current;
            string ReadmeText = String.Format("Buen dia!\r\n" +
                                 "Este archivo fue generado dinamicamente por nuestro sistema\r\n" +
                                 "Fecha y hora de generacion : {0}\r\n" +
                                 "Para su seguridad este archivo esta encriptado\r\n" +
                                 "Para abrir utilizar el password: {3}\r\n" +
                                 "Tipo de encriptacion utilizado: {4}\r\n" +
                                 "Cualquier problema favor de cominicarse con el area de TI\r\n" +
                                 "o enviar correo a soporte@proeci.com.mx\r\n" +
                                 "Gracias por su preferencia\r\n" +
                                 "Attn:\r\n" +
                                 "Area de Desarrollo de Software\r\n" +
                                 "GRUPO PROECI\r\n" +
                                 "----------------------------------------\r\n",
                                 System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                 System.Environment.MachineName,
                                 c.Request.ServerVariables["SERVER_SOFTWARE"],
                                 "proeci",
                                 EncryptionAlgorithm.WinZipAes256.ToString());
            string tempfile = String.Format("{0}_{1}.zip", _ref, Guid.NewGuid());
            string archiveName = System.Web.HttpContext.Current.Server.MapPath("~/tempdigitalfiles/") + tempfile;

            if (System.IO.File.Exists(archiveName))
                System.IO.File.Delete(archiveName);

            string newFileNamex = string.Empty;
            using (ZipFile zip = new ZipFile())
            {
                char[] charSeparators = new char[] { '|' };
                string[] result = id_docfilename.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                string[] resultfull = fullfilenames.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                for (int count = 0; count < result.Length; count++)
                {
                    string filename = string.Empty;
                    BsonValue myId_pdf;
                    MongoCollection<BsonDocument> _collection = _db.GetCollection<BsonDocument>(CollectionName);
                    var query = Query.EQ("idfilename", result[count]);
                    var query_id = Query.EQ("idfilename", ObjectId.Parse(result[count]));
                    foreach (var document in _collection.Find(query_id))
                    {
                        myId_pdf = (BsonValue)document["idfilename"];
                        newFileNamex = System.Web.HttpContext.Current.Server.MapPath("~/tempdigitalfiles/") + resultfull[count] + "." + document["content_type"].AsString;
                        var file = _db.GridFS.FindOne(Query.EQ("_id", myId_pdf));
                        using (var stream = file.OpenRead())
                        {
                            var bytes = new byte[stream.Length];
                            stream.Read(bytes, 0, (int)stream.Length);
                            using (var newFs = new System.IO.FileStream(newFileNamex, System.IO.FileMode.Create))
                            {
                                newFs.Write(bytes, 0, bytes.Length);
                            }
                        }
                        zip.AddFile(newFileNamex, "Expediente");
                    }
                }
                zip.Save(archiveName);
                zip.Dispose();
            }
            if (mailto != "")
            {
                core.EmailHandler newmail = new core.EmailHandler();
                string subject = "GRUPO PROECI EXPEDIENTE DIGITAL";
                string boddy = "Buen dia, anexo encontra los archivos solicitados en nuestro expediente digital";
                boddy += "<br />";
                boddy += "Gracias por su preferencia";
                boddy += "<br />";
                boddy += "<br />";
                boddy += "GRUPO PROECI";
                boddy += "<br />";
                boddy += "<br />";
                boddy += "Cualqiuier problema de acceso favor de notificarle al <a href='mailto:lgonzalez@sanmateo-fwd.com' target='_self' >administrador del sistema</a>";
                boddy += "<br />";
                boddy += "<br />";
                boddy += "Avisos de Dashboard, este es un correo automatizado, favor de no reesponder...";
                newmail.SendMailMessage(mailto, subject, boddy, false, false, archiveName);
            }
            return System.IO.Path.GetFileName(archiveName);
        }
        public IList<newProfile> GetArrayData(string referencia, string facturas, string consolidados, string preentrada)
        {
            BsonValue myId_pdf;
            MongoCollection<BsonDocument> _collection = _db.GetCollection<BsonDocument>("Documents");

            List<string> list = new List<string>();
            list.Add(referencia);

            string qry = referencia;
            char[] charSeparators = new char[] { '|' };
            string[] result = facturas.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
            for (int count = 0; count < result.Length; count++)
                list.Add(result[count]);

            result = consolidados.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
            for (int count = 0; count < result.Length; count++)
                list.Add(result[count]);

            if (preentrada != "")
                list.Add(preentrada);

            var sort = SortBy.Descending("doctype");
            var query = MongoDB.Driver.Builders.Query.In("key", BsonArray.Create(list));

            var cursor = _collection.Find(query);
            int i = 0;
            IList<newProfile> doclist = new List<newProfile>();
            foreach (var sdoc in cursor)
            {
                string json = string.Empty;
                myId_pdf = (BsonValue)sdoc["idfilename"];
                newProfile mydoc = new newProfile();
                mydoc.Id = sdoc["_id"].ToString();
                mydoc.key = getArrayValuesToString(sdoc["key"].AsBsonArray);
                mydoc.key2 = getArrayValuesToString(sdoc["key2"].AsBsonArray);
                mydoc.content_type = sdoc["content_type"].AsString;
                mydoc.idfilename = sdoc["idfilename"].ToString();
                mydoc.filename = sdoc["filename"].ToString();
                mydoc.lenght = sdoc["lenght"].ToDouble();
                mydoc.uploaddate = sdoc["uploaddate"].ToString();
                mydoc.doctype = sdoc["doctype"].ToString();
                mydoc.doctypename = GetCollDataFieldValue("DocTypes", "code", mydoc.doctype, "name");
                if (mydoc.doctypename == "")
                    mydoc.doctypename = "NO DEFINIDO";

                mydoc.docdate = sdoc["docdate"].ToString();
                mydoc.expdate = sdoc.GetValue("dateto", "N/A").ToString();
                mydoc.tags = getArrayValuesToString(sdoc["tags"].AsBsonArray);
                mydoc.year = sdoc["year"].ToInt32();
                mydoc.month = sdoc["month"].ToInt32();
                mydoc.day = sdoc["day"].ToInt32();
                mydoc.source = sdoc.GetValue("source", "").ToString();
                mydoc.comments = sdoc["comments"].ToString();
                mydoc.user = sdoc.GetValue("user", "N/A").ToString();
                doclist.Add(mydoc);
                i++;
                try
                {
                    thumbFileId(myId_pdf, mydoc.content_type.ToString());
                }
                catch (Exception ex)
                {
                    //throw new Exception(ex.Message);
                }

            }
            return doclist;
        }
        public IList<DocTypes> GetDocumentsList()
        {

            IList<DocTypes> doclist = new List<DocTypes>();
            //if (!System.Web.HttpContext.Current.IsDebuggingEnabled)
            //{
            MongoCollection<BsonDocument> _collection = _db.GetCollection<BsonDocument>("DocTypes");
            var sort = SortBy.Ascending("name");
            var query = Query.EQ("enable", "1");
            var cursor = _collection.Find(query).SetSortOrder(sort);
            string filename = string.Empty;
            foreach (var sdoc in cursor)
            {
                DocTypes newdoc = new DocTypes();
                newdoc.docname = sdoc["name"].ToString();
                newdoc.doccode = sdoc["code"].ToString();
                doclist.Add(newdoc);
            }
            //}
            return doclist;
        }
        public string[] ValidateCollData(string CollectionName, string searchKey, System.Data.DataRow rw)
        {
            MongoCollection<BsonDocument> _collection = _db.GetCollection<BsonDocument>(CollectionName);
            var query = Query<Documents>.EQ(e => e.key, searchKey);
            var cursor = _collection.Find(query);
            var collection = _db.GetCollection<BsonDocument>(CollectionName);
            int i = 0;

            string[,] docs = new string[24, 2];
            docs[0, 0] = "FOTO MERCANCIA";
            docs[1, 0] = "BL";
            docs[2, 0] = "CUENTA AMERICANA";
            docs[3, 0] = "FACTURA";
            docs[4, 0] = "LISTA DE EMPAQUE";
            docs[5, 0] = "CARTA NOM";
            docs[6, 0] = "OTROS ANEXOS";
            docs[7, 0] = "SOLICITUD DE IMPUESTOS";
            docs[8, 0] = "NOTA DE REVISION";
            docs[9, 0] = "DEPOSITO";
            docs[10, 0] = "ORDEN CARGA";
            docs[11, 0] = "REMISION";
            docs[12, 0] = "PEDIMENTO COMPLETO";
            docs[13, 0] = "PEDIMENTO SIMPLIFICADO";
            docs[14, 0] = "PEDIMENTO ANTERIOR";
            docs[15, 0] = "HOJA DE CALCULO";
            docs[16, 0] = "MANIFESTACION DE VALOR";
            docs[17, 0] = "CUENTA MEXICANA";
            docs[18, 0] = "CUENTA MEXICANA XML";
            docs[19, 0] = "COVE ACUSE";
            docs[20, 0] = "COVE XML ENVIO";
            docs[22, 0] = "COVE DETALLE";
            docs[23, 0] = "VU EDOCUMENT";

            docs[0, 1] = "D001";
            docs[1, 1] = "D004";
            docs[2, 1] = "D010";
            docs[3, 1] = "D006";
            docs[4, 1] = "D023";
            docs[5, 1] = "D018";
            docs[6, 1] = "D022";
            docs[7, 1] = "D103";
            docs[8, 1] = "D102";
            docs[9, 1] = "D027";
            docs[10, 1] = "D104";
            docs[11, 1] = "D105";
            docs[12, 1] = "D201";
            docs[13, 1] = "D202";
            docs[14, 1] = "D205";
            docs[15, 1] = "D203";
            docs[16, 1] = "D204";
            docs[17, 1] = "D011";
            docs[18, 1] = "D012";
            docs[19, 1] = "D301";
            docs[20, 1] = "D302";
            docs[21, 1] = "D304";
            docs[22, 1] = "D303";

            string doctype;
            foreach (var sdoc in cursor)
            {
                doctype = sdoc["doctype"].ToString();
                for (int x = 0; x <= 22; x++)
                {
                    if (doctype == docs[x, 1])
                    {
                        i++;
                        rw["d" + x] = 1;
                        break;
                    }
                    else if (doctype == "D216")
                    {
                        rw["d15"] = 1;
                        break;
                    }
                    else if (doctype == "D211")
                    {
                        rw["d16"] = 1;
                        break;
                    }
                }
            }
            string[] valores = new string[2];
            return valores;
        }
        private string QueryFileId(BsonValue myId, string ext, string _ref)
        {
            string newFileName = System.Web.HttpContext.Current.Server.MapPath("~/tempdigitalfiles/") + myId.ToString() + "." + ext;
            if (System.IO.File.Exists(newFileName))
                System.IO.File.Delete(newFileName);

            var file = _db.GridFS.FindOne(Query.EQ("_id", myId));
            using (var stream = file.OpenRead())
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);
                using (var newFs = new System.IO.FileStream(newFileName, System.IO.FileMode.CreateNew))
                {
                    newFs.Write(bytes, 0, bytes.Length);
                }
            }
            return newFileName;
        }
        private void thumbFileId(BsonValue myId, string ext)
        {
            ext = ext.ToLower();
            string newFileName = System.Web.HttpContext.Current.Server.MapPath("~/tempdigitalfiles/") + myId.ToString() + "." + ext.Replace(".", "");
            string thumbfile = System.Web.HttpContext.Current.Server.MapPath("~/thumbs/") + "thumb_" + myId.ToString() + ".jpeg";
            if (!System.IO.File.Exists(thumbfile))
            {
                if (ext == "pdf" || ext == "jpg" || ext == "png" || ext == ".png" || ext == "bmp" || ext == "gif" || ext == "tiff")
                {
                    if (!System.IO.File.Exists(newFileName))
                    {
                        var file = _db.GridFS.FindOne(Query.EQ("_id", myId));
                        using (var stream = file.OpenRead())
                        {
                            var bytes = new byte[stream.Length];
                            stream.Read(bytes, 0, (int)stream.Length);
                            using (var newFs = new System.IO.FileStream(newFileName, System.IO.FileMode.Create))
                            {
                                newFs.Write(bytes, 0, bytes.Length);
                            }
                        }
                    }
                    if (ext == "pdf")
                    {
                        ImageMagick.MagickImage im = new ImageMagick.MagickImage(newFileName);
                        im.Quality = 20;
                        im.CompressionMethod = ImageMagick.CompressionMethod.JPEG;
                        im.Write(thumbfile);
                    }
                    else
                    {
                        using (System.Drawing.Image bigImage = new System.Drawing.Bitmap(newFileName))
                        {
                            using (System.Drawing.Image smallImage = bigImage.GetThumbnailImage(120, 120, null, IntPtr.Zero))
                            {
                                smallImage.Save(thumbfile, System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                        }
                    }
                    System.IO.File.Delete(newFileName);
                }
            }
        }
        private string QueryZipFileId(BsonValue myId, string ext, string fullfilename, string _ref)
        {
            string newFileName = System.Web.HttpContext.Current.Server.MapPath("~/tempdigitalfiles/") + fullfilename + "." + ext;
            var file = _db.GridFS.FindOne(Query.EQ("_id", myId));

            using (var stream = file.OpenRead())
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);
                using (var newFs = new System.IO.FileStream(newFileName, System.IO.FileMode.Create))
                {
                    newFs.Write(bytes, 0, bytes.Length);
                }
            }
            System.Web.HttpContext c = System.Web.HttpContext.Current;
            string ReadmeText = String.Format("Buen dia!\r\n" +
                                 "Este archivo fue generado dinamicamente por nuestro sistema\r\n" +
                                 "Fecha y hora de generacion : {0}\r\n" +
                                 "Para su seguridad este archivo esta encriptado\r\n" +
                                 "Para abrir utilizar el password: {3}\r\n" +
                                 "Tipo de encriptacion utilizado: {4}\r\n" +
                                 "Cualquier problema favor de cominicarse con el area de TI\r\n" +
                                 "o enviar correo a soporte@proeci.com.mx\r\n" +
                                 "Gracias por su preferencia\r\n" +
                                 "Attn:\r\n" +
                                 "Area de Desarrollo de Software\r\n" +
                                 "GRUPO PROECI\r\n" +
                                 "----------------------------------------\r\n",
                                 System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                 System.Environment.MachineName,
                                 c.Request.ServerVariables["SERVER_SOFTWARE"],
                                 "proeci",
                                 EncryptionAlgorithm.WinZipAes256.ToString());
            string archiveName = String.Format("{0}.zip", _ref);
            string tempfile = System.Web.HttpContext.Current.Server.MapPath("~/tempdigitalfiles/") + archiveName;

            //Elimina archivo si existe
            if (System.IO.File.Exists(tempfile))
                System.IO.File.Delete(tempfile);

            using (ZipFile zip = new ZipFile())
            {
                //zip.AddEntry("instrucciones.txt", ReadmeText, Encoding.Default);
                //zip.Password = "proeci";
                //zip.Encryption = EncryptionAlgorithm.WinZipAes256;
                zip.AddFile(newFileName, "Expediente");
                //zip.AddFile(newFileName);
                zip.Save(tempfile);
            }
            return archiveName;
        }
        private string getArrayValuesToString(BsonArray dataArray)
        {
            string Values = String.Empty;
            int length = dataArray.Count();
            if (length > 0)
                for (int i = 0; i < length; i++)
                    Values += dataArray[i].ToString();

            return Values;
        }
        private string GetCollDataFieldValue(string CollectionName, string searchKey, string searchValue, string returnKey)
        {
            MongoCollection<BsonDocument> _collection = _db.GetCollection<BsonDocument>(CollectionName);
            var query = Query.EQ(searchKey, searchValue);
            var cursor = _collection.Find(query);
            var first = cursor.FirstOrDefault();
            if (first == null)
                return string.Empty;
            else
                return first[returnKey].ToString();
        }
        public void DeleteCollData(string CollectionName, string searchKey, string id_docfilename, string user)
        {
            string newFileName = string.Empty;
            BsonValue myId_pdf;
            MongoCollection<BsonDocument> _collection = _db.GetCollection<BsonDocument>(CollectionName);
            var query = Query.EQ("idfilename", id_docfilename);
            var query_id = Query.EQ("idfilename", ObjectId.Parse(id_docfilename));
            var cursor = _collection.Find(query_id);
            foreach (var sdoc in cursor)
            {
                myId_pdf = (BsonValue)sdoc["idfilename"];
                _db.GridFS.Delete(Query.EQ("_id", myId_pdf));
            }
            _collection.Remove(query_id);
            InsertDeleteLog(_db, searchKey, id_docfilename, "", user);
        }
        public string SubirReferencia(string sRef, string fac, string sRef2, string sNotas, string sOrigin, string tipo, string localpath)
        {
            string _msg = String.Empty;
            BsonArray _extraKey2;
            //Consulta datos
            if (sRef.Length == 0 || sRef == "SINASIGNAR")
                _extraKey2 = GetDataFromAduanaFac(fac, sRef2);
            else
                _extraKey2 = GetDataFromAduana(sRef, sRef2);

            //sube documento
            try
            {
                if (sRef.Length == 0 || sRef == "SINASIGNAR")
                    SaveDocument(sRef2, _extraKey2, sNotas, sOrigin, "../uploadfiles/" + localpath, tipo, false, false, "", "", "", "");
                else
                    SaveDocument(sRef, _extraKey2, sNotas, sOrigin, "../uploadfiles/" + localpath, tipo, false, false, "", "", "", "");

                //Delete the temp file
                DeleteFile(localpath);
                _msg = "Documento grabado con exito";
            }
            catch (Exception ex)
            {
                _msg = "Error al grabar documento! " + ex.Message.ToString();
            }
            return _msg;
        }
        public void SaveDocument(string sKey, BsonArray sKey2_extra, string sNotas, string sOrigin, string localfile, string tipodoc, bool clientaccess, bool expires, string datefrom, string dateto, string ext, string user)
        {

            MongoClient _client;
            MongoServer _server;
            MongoDatabase _db;

            string connectionstring = "mongodb://" + mongoHost + ":" + mongoPort;
            _client = new MongoClient(connectionstring);
            _server = _client.GetServer();
            _db = _server.GetDatabase("digital");

            InsertDataDocumentos(_db, sKey, sKey2_extra, sNotas, sOrigin, localfile, tipodoc, clientaccess, expires, datefrom, dateto, ext, user);

        }
        private MongoGridFSFileInfo UploadFile(MongoDatabase _db, string localfileName, string datafileName)
        {
            BsonValue fileId;
            MongoGridFSFileInfo gridFsInfo;

            using (var fs = new System.IO.FileStream(localfileName, System.IO.FileMode.Open))
            {
                _db.GridFS.Delete(datafileName);
                gridFsInfo = _db.GridFS.Upload(fs, datafileName);
                fileId = gridFsInfo.Id;
            }
            return gridFsInfo;
        }
        private void InsertDataDocumentos(MongoDatabase _db, string sKey, BsonArray sKey2_extra, string sNotas, string sOrigin, string localfile, string tipodoc, bool clientaccess, bool expires, string datefrom, string dateto, string ext, string user)
        {
            MongoCollection<BsonDocument> colldocuments = _db.GetCollection<BsonDocument>("Documents");
            string localfile_complete = System.Web.HttpContext.Current.Server.MapPath(localfile);
            MongoGridFSFileInfo gridFsInfo = UploadFile(_db, localfile_complete, getdatafileName(tipodoc));
            if (ext == "")
                ext = System.IO.Path.GetExtension(localfile).Replace(".", "");

            BsonDocument[] mydata = {
                new BsonDocument {
                    { "key", new BsonArray {  sKey } },     
                    { "key2",  sKey2_extra  },              
                    { "content_type", ext },                
                    { "idfilename", gridFsInfo.Id },        
                    { "filename",  gridFsInfo.Name },       
                    { "lenght", gridFsInfo.Length },        
                    { "uploaddate",  DateTime.UtcNow },        
                    { "uploaddatenew",  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },        
                    { "doctype", tipodoc },                 
                    { "docdate", DateTime.Now },            
                    { "docdatenew", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },            
                    { "tags", new BsonArray { "p1", "p2","p3","p4", "p5","p6" } },  
                    { "year",  DateTime.Now.Year },         
                    { "month",  DateTime.Now.Month },       
                    { "day",  DateTime.Now.Day },           
                    { "source", sOrigin },                  
                    { "comments", sNotas },
                    { "clientaccess", clientaccess },
                    { "expires", expires },
                    { "datefrom", datefrom },
                    { "dateto", dateto },
                    { "user", user }
                }
            };
            colldocuments.InsertBatch(mydata);
        }
        private void InsertDeleteLog(MongoDatabase _db, string sKey, string idorg, string doctype, string user)
        {
            MongoCollection<BsonDocument> colldocuments = _db.GetCollection<BsonDocument>("Systemlog");

            BsonDocument[] mydata = {
                new BsonDocument {
                    { "key", new BsonArray {  sKey } },     
                    { "action",  "DEL"  },              
                    { "logdate",  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },        
                    { "idoriginal", idorg },                 
                    { "doctype", doctype },            
                    { "admin", "admin" },
                    { "user", user }
                }
            };
            colldocuments.InsertBatch(mydata);
        }
        public void SubirReferenciaFoto(string sRef, string sNotas, string sOrigin, string tipo, string localpath, bool clientaccess, bool expires, string datefrom, string dateto, string ext, string user)
        {
            string _msg = String.Empty;
            BsonArray _extraKey2;

            //Consulta datos
            _extraKey2 = GetDataFromAduana(sRef, "");
            try
            {
                string sfileName2pdf = System.IO.Path.GetFileNameWithoutExtension(localpath) + ".pdf"; //localpath.Substring(0, localpath.Length - 3) + "pdf";

                //Convierte a PDF 
                string SavedLocation = System.Web.HttpContext.Current.Server.MapPath(@"../uploadfiles") + "\\" + localpath;
                string PDFSavedLocation = System.Web.HttpContext.Current.Server.MapPath(@"../uploadfiles") + "\\" + sfileName2pdf;
                convertir_PDF(SavedLocation, PDFSavedLocation);
                //Save into DB
                SaveDocument(sRef, _extraKey2, sNotas, sOrigin, "../uploadfiles/" + sfileName2pdf, tipo, clientaccess, expires, datefrom, dateto, "pdf", user);
                //Delete the temp file
                DeleteFile(localpath);
                _msg = "Documento grabado con exito";
            }
            catch (Exception ex)
            {
                _msg = "Error al grabar documento! " + ex.Message.ToString();
            }

        }
        private void convertir_PDF(string source_img_file, string destination_pdf_file)
        {
            iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.LETTER.Rotate(), 0, 0, 0, 0);
            using (var stream = new FileStream(destination_pdf_file, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                PdfWriter.GetInstance(document, stream);
                document.Open();
                using (var imageStream = new FileStream(source_img_file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var image = iTextSharp.text.Image.GetInstance(imageStream);
                    image.ScaleToFit(document.PageSize.Width, document.PageSize.Height);
                    document.Add(image);
                }
                document.Close();
            }
        }
        public string UploadByRef(string sRef, string sNotas, string sOrigin, string tipo, string localpath, bool clientaccess, bool expires, string datefrom, string dateto, string ext, string user)
        {
            string _msg = string.Empty;
            BsonArray _extraKey2 = new BsonArray();
            //Consulta datos
            if (sRef.Length > 0)
                _extraKey2 = GetDataFromAduana(sRef, sRef);
            //sube documento
            try
            {
                if (sRef.Length > 0)
                    SaveDocument(sRef, _extraKey2, sNotas, sOrigin, "../uploadfiles/" + localpath, tipo, clientaccess, expires, datefrom, dateto, ext, user);

                //Delete the temp file
                DeleteFile(localpath);
                _msg = "Documento grabado con exito";
            }
            catch (Exception ex)
            {
                _msg = "Error al grabar documento! " + ex.Message.ToString();
            }

            return _msg;
        }
        public string UploadByRef(string sRef, string sNotas, string sOrigin, string tipo, string localpath, bool clientaccess, bool expires, string datefrom, string dateto, string ext, string user, bool getfullpath)
        {
            string _msg = string.Empty;
            BsonArray _extraKey2 = new BsonArray();
            //Consulta datos
            if (sRef.Length > 0)
                _extraKey2 = GetDataFromAduana(sRef, sRef);
            //sube documento
            try
            {
                if (sRef.Length > 0)
                {
                    SaveDocument(sRef, _extraKey2, sNotas, sOrigin, "../pdf/" + localpath, tipo, clientaccess, expires, datefrom, dateto, ext, user);
                }

                //Delete the temp file
                DeleteFile(localpath);
                _msg = "Documento grabado con exito";
            }
            catch (Exception ex)
            {
                _msg = "Error al grabar documento! " + ex.Message.ToString();
            }

            return _msg;
        }
        private string getdatafileName(string sDOcType)
        {
            string s = string.Format("{0}_{1:yyyyMMddhhmmss}", sDOcType, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            s = string.Format("{0}_{1}", sDOcType, Guid.NewGuid());
            return s;
        }
        private void DeleteFile(string filename)
        {
            if (!System.Configuration.ConfigurationManager.AppSettings["deleteAfterProcessed"].ToString().Equals("true"))
                return;

            string fileLocation = System.Web.HttpContext.Current.Server.MapPath("../uploadfiles/") + "\\" + filename;
            if (System.IO.File.Exists(fileLocation))
            {
                try
                {
                    System.IO.File.Delete(fileLocation);
                }
                catch (System.IO.IOException e)
                {
                    string _error = e.Message;
                }
            }

            //Delete the JPG if it exists.
            fileLocation = System.Web.HttpContext.Current.Server.MapPath("upload") + "\\" + filename.Substring(0, filename.Length - 3) + "jpg";
            if (System.IO.File.Exists(fileLocation))
            {
                try
                {
                    System.IO.File.Delete(fileLocation);
                }
                catch (System.IO.IOException e)
                {
                    string _error = e.Message;
                }
            }
        }
        string AduanaConnection = System.Configuration.ConfigurationManager.ConnectionStrings["default"].ToString();

        public BsonArray GetDataFromAduana(string sRef, string sKey2)
        {
            string sKeyExtra = String.Empty;
            BsonArray originalTags = new BsonArray();
            core.extendsql sqlService = new core.extendsql();
            sqlService.cmdType = core.extendsql.CommandTypes.TXT;
            sqlService.cnnName = core.extendsql.ConnectionNames.def;
            sqlService.sqltxt = "select c.rfc, t.tracli, p.proIRS, t.traprocli, t.trareferencia, t.traaduana, t.trapedimento, t.pref, b.bodnopedido from Trafico t " +
                           " left join clientes c on c.cliente_id = t.tracli " +
                           " left join procli p on p.proveedor_id = t.traprocli " +
                           " left join tblbod b on b.bodreferencia = t.trareferencia " +
                           " where trareferencia = '" + sRef + "'";
            DataTable ds = new DataTable();
            ds = sqlService.GetAsyncDataTableMethod();
            foreach (DataRow dr in ds.Rows)
            {
                originalTags = new BsonArray { 
                    "d_" + sKey2, 
                    "rfc_" + ( String.IsNullOrEmpty(dr["rfc"].ToString().Trim()) ? "" : dr["rfc"].ToString().Trim() ),  
                    "c_" + ( String.IsNullOrEmpty( dr["tracli"].ToString()) ? "" : dr["tracli"].ToString().Trim() ),   
                    "irs_" + ( String.IsNullOrEmpty(dr["proIRS"].ToString().Trim()) ? "" : dr["proIRS"].ToString().Trim() ),   
                    "p_" + ( String.IsNullOrEmpty(dr["traprocli"].ToString()) ? "" : dr["traprocli"].ToString().Trim() ),  
                    "oc_" + ( String.IsNullOrEmpty(dr["bodnopedido"].ToString().Trim()) ? "" : dr["bodnopedido"].ToString().Trim() ),   
                    "adu_" + ( String.IsNullOrEmpty(dr["traaduana"].ToString()) ? "" : dr["traaduana"].ToString().Trim() ),   
                    "ped_" + ( String.IsNullOrEmpty(dr["trapedimento"].ToString()) ? "" : dr["trapedimento"].ToString().Trim() ),  
                    "pref_" + ( String.IsNullOrEmpty(dr["pref"].ToString()) ? "" : dr["pref"].ToString().Trim() )
                 };
            }
            return originalTags;
        }

        public BsonArray GetDataFromAduanaFac(string sRef, string sKey2)
        {
            string sKeyExtra = String.Empty;
            BsonArray originalTags = new BsonArray();
            string sCmd = "select c.rfc, t.tracli, p.proIRS, t.traprocli, t.trareferencia, t.traaduana, t.trapedimento, t.pref, b.bodnopedido from Trafico t " +
                           " left join clientes c on c.cliente_id = t.tracli " +
                           " left join procli p on p.proveedor_id = t.traprocli " +
                           " left join tblbod b on b.bodreferencia = t.trareferencia " +
                           " where trareferencia = '" + sRef + "'";
            sCmd = @"select c.rfc, tblfactgen.facgcli, p.proIRS, tblfactgen.facgprov,t.trareferencia, t.traaduana, t.trapedimento, tblfactgen.pref, b.bodnopedido 
                        from tblfactgen
                        LEFT JOIN Trafico t  ON t.trareferencia=tblfactgen.facgref
                        left join clientes c on c.cliente_id = tblfactgen.facgcli  
                        left join procli p on p.proveedor_id = tblfactgen.facgprov
                        left join tblbod b on b.bodreferencia = t.trareferencia  
                        where facgnofac = '" + sRef + "'";

            core.extendsql sqlService = new core.extendsql();
            sqlService.cmdType = core.extendsql.CommandTypes.TXT;
            sqlService.cnnName = core.extendsql.ConnectionNames.def;
            sqlService.sqltxt = sCmd;
            DataTable ds = new DataTable();
            ds = sqlService.GetAsyncDataTableMethod();

            foreach (DataRow dr in ds.Rows)
            {
                originalTags = new BsonArray { 
                    "d_" + sKey2, 
                    "rfc_" + ( String.IsNullOrEmpty(dr["rfc"].ToString().Trim()) ? "" : dr["rfc"].ToString().Trim() ),  
                    "c_" + ( String.IsNullOrEmpty( dr["facgcli"].ToString()) ? "" : dr["facgcli"].ToString().Trim() ),   
                    "irs_" + ( String.IsNullOrEmpty(dr["proIRS"].ToString().Trim()) ? "" : dr["proIRS"].ToString().Trim() ),   
                    "p_" + ( String.IsNullOrEmpty(dr["facgprov"].ToString()) ? "" : dr["facgprov"].ToString().Trim() ),  
                    "oc_" + ( String.IsNullOrEmpty(dr["bodnopedido"].ToString().Trim()) ? "" : dr["bodnopedido"].ToString().Trim() ),   
                    "adu_" + ( String.IsNullOrEmpty(dr["traaduana"].ToString()) ? "" : dr["traaduana"].ToString().Trim() ),   
                    "ped_" + ( String.IsNullOrEmpty(dr["trapedimento"].ToString()) ? "" : dr["trapedimento"].ToString().Trim() ),  
                    "pref_" + ( String.IsNullOrEmpty(dr["pref"].ToString()) ? "" : dr["pref"].ToString().Trim() )
                 };
            }

            return originalTags;
        }
        public string zipfullexp(string referencia, string facturas, string consolidados, string preentrada)
        {
            BsonValue myId_pdf;
            MongoCollection<BsonDocument> _collection = _db.GetCollection<BsonDocument>("Documents");

            List<string> list = new List<string>();
            list.Add(referencia);

            string qry = referencia;
            char[] charSeparators = new char[] { '|' };
            string[] result = facturas.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
            for (int count = 0; count < result.Length; count++)
                list.Add(result[count]);

            result = consolidados.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
            for (int count = 0; count < result.Length; count++)
                list.Add(result[count]);

            if (preentrada != "")
                list.Add(preentrada);

            var sort = SortBy.Descending("doctype");
            var query = MongoDB.Driver.Builders.Query.In("key", BsonArray.Create(list));

            var cursor = _collection.Find(query);
            int i = 0;
            List<newdocument> generatedfiles = new List<newdocument>();
            IList<newProfile> doclist = new List<newProfile>();
            foreach (var sdoc in cursor)
            {
                myId_pdf = (BsonValue)sdoc["idfilename"];

                var doctypename = GetCollDataFieldValue("DocTypes", "code", sdoc["doctype"].ToString(), "name"); ;

                newdocument newdoc = new newdocument();
                newdoc.fileid = myId_pdf;
                newdoc.reference = sdoc["key"].ToString();
                newdoc.savedFileName = sdoc["key"].ToString() + "_" + doctypename + "_" + sdoc["idfilename"].ToString() + "." + sdoc["content_type"].ToString();
                newdoc.fileExtension = sdoc["content_type"].ToString();

                generatedfiles.Add(newdoc);
            }

            return GetFullintoZip(generatedfiles, referencia);
        }
        public string zipforcuentagastos(string keyref, string encref, string mailto, string txtsubject, bool mergefiles, bool compressfiles, string archivo, string facturas, string consolidados, string preentrada)
        {
            List<newdocument> generatedfiles = new List<newdocument>();
            List<validdocs> doclist = new List<validdocs>();
            doclist.Add(new validdocs("ANEXOS HCyMV", "D207"));
            doclist.Add(new validdocs("COMPRA DOLARES", "D601"));
            doclist.Add(new validdocs("COMPROBADOS EN PUERTO", "D056"));
            doclist.Add(new validdocs("COMPROBANTE DE ARRASTRE", "D003"));
            doclist.Add(new validdocs("COMPROBANTE PAGO BANCO", "D044"));
            doclist.Add(new validdocs("CUENTA AMERICANA", "D010"));
            doclist.Add(new validdocs("CUENTA MEXICANA", "D011"));
            doclist.Add(new validdocs("DEPOSITO", "D027"));
            doclist.Add(new validdocs("FACTURA CRUCE (TRANSFER)", "D068"));
            doclist.Add(new validdocs("FACTURA FLETE MARITIMO", "D604"));
            doclist.Add(new validdocs("FACTURA LOGISTICA ZEBRA", "D606"));
            doclist.Add(new validdocs("FACTURA LOGISTICA", "D049"));
            doclist.Add(new validdocs("FACTURA MANIOBRAS PDF", "D032"));
            doclist.Add(new validdocs("FACTURA TRANSPORTE", "D031"));
            doclist.Add(new validdocs("HOJA DE CALCULO", "D203"));
            doclist.Add(new validdocs("HOJA DE CALCULO", "D216"));
            doclist.Add(new validdocs("INSPECCIONES PDF", "D034"));
            doclist.Add(new validdocs("INSPECCIONES XML", "D035"));
            doclist.Add(new validdocs("MANIFESTACION DE VALOR", "D204"));
            doclist.Add(new validdocs("MANIFESTACION DE VALOR", "D211"));
            doclist.Add(new validdocs("MANIOBRAS XML", "D033"));
            doclist.Add(new validdocs("OTROS COMPROBANTES", "D021"));
            doclist.Add(new validdocs("PAGO DE DERECHOS", "D053"));
            doclist.Add(new validdocs("PEDIMENTO ANTERIOR", "D205"));
            doclist.Add(new validdocs("PEDIMENTO ANTERIOR", "D206"));
            doclist.Add(new validdocs("PEDIMENTO COMPLETO", "D201"));
            doclist.Add(new validdocs("PEDIMENTO SIMPLIFICADO", "D202"));
            doclist.Add(new validdocs("XML CUENTA GASTOS", "D012"));
            doclist.Add(new validdocs("XML FACTURA CRUCE (TRANSFER)", "D069"));
            doclist.Add(new validdocs("XML FACTURA FLETE MARITIMO", "D605"));
            doclist.Add(new validdocs("XML FACTURA LOGISTICA ZEBRA", "D607"));
            doclist.Add(new validdocs("XML FACTURA LOGISTICA", "D050"));
            doclist.Add(new validdocs("XML FACTURA TRANSPORTE", "D603"));
            doclist.Add(new validdocs("XML OTROS GASTOS", "D060"));
            doclist.Add(new validdocs("TRAFICO-EXPEDIENTE", "D111"));


            BsonValue myId_pdf;
            MongoCollection<BsonDocument> _collection = _db.GetCollection<BsonDocument>("Documents");
            List<string> list = new List<string>();
            list.Add(keyref);

            char[] charSeparators = new char[] { '|' };
            string[] result = facturas.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
            for (int count = 0; count < result.Length; count++)
                list.Add(result[count]);

            result = consolidados.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
            for (int count = 0; count < result.Length; count++)
                list.Add(result[count]);

            if (preentrada != "")
                list.Add(preentrada);

            var sort = SortBy.Descending("doctype");
            var validtypes = new List<string>();
            foreach (var x in doclist)
            {
                validtypes.Add(x.docid);
            }

            var qType1 = Query.And(Query.In("key", BsonArray.Create(list)), Query.In("doctype", BsonArray.Create(validtypes)));

            var cursor = _collection.Find(qType1);
            foreach (var sdoc in cursor)
            {
                myId_pdf = (BsonValue)sdoc["idfilename"];

                var doctypename = doclist.Find(p => p.docid == sdoc["doctype"].ToString());

                newdocument newdoc = new newdocument();
                newdoc.fileid = myId_pdf;
                newdoc.reference = sdoc["key"].ToString();
                newdoc.savedFileName = sdoc["key"].ToString() + "_" + doctypename.docname + "_" + sdoc["idfilename"].ToString() + "." + sdoc["content_type"].ToString();
                newdoc.fileExtension = sdoc["content_type"].ToString();

                if (sdoc["doctype"].ToString() == "D012" || sdoc["doctype"].ToString() == "D011")
                    newdoc.savedFileName = archivo + "." + sdoc["content_type"].ToString();

                generatedfiles.Add(newdoc);
            }

            if (generatedfiles.Count > 0)
            {
                GetSelectedintoZipCtasGastos(generatedfiles, keyref, mailto, encref, txtsubject, mergefiles, compressfiles);
            }

            return "";
        }
        public List<validdocs> getExpedientes(List<string> refs)
        {

            List<validdocs> doclist = new List<validdocs>();
            doclist.Add(new validdocs("FOTO MERCANCIA", "D001"));
            doclist.Add(new validdocs("BL", "D004"));
            doclist.Add(new validdocs("CUENTA AMERICANA", "D010"));
            doclist.Add(new validdocs("FACTURA", "D006"));
            doclist.Add(new validdocs("LISTA DE EMPAQUE", "D023"));
            doclist.Add(new validdocs("CARTA NOM", "D018"));
            doclist.Add(new validdocs("OTROS ANEXOS", "D022"));
            doclist.Add(new validdocs("SOLICITUD DE IMPUESTOS", "D103"));
            doclist.Add(new validdocs("NOTA DE REVISION", "D102"));
            doclist.Add(new validdocs("DEPOSITO", "D027"));
            doclist.Add(new validdocs("ORDEN CARGA", "D104"));
            doclist.Add(new validdocs("REMISION", "D105"));
            doclist.Add(new validdocs("PEDIMENTO COMPLETO", "D201"));
            doclist.Add(new validdocs("PEDIMENTO SIMPLIFICADO", "D202"));
            doclist.Add(new validdocs("PEDIMENTO ANTERIOR", "D205"));
            doclist.Add(new validdocs("HOJA DE CALCULO", "D203"));
            doclist.Add(new validdocs("MANIFESTACION DE VALOR", "D204"));
            doclist.Add(new validdocs("CUENTA MEXICANA", "D011"));
            doclist.Add(new validdocs("CUENTA MEXICANA XML", "D012"));
            doclist.Add(new validdocs("COVE ACUSE", "D301"));
            doclist.Add(new validdocs("COVE XML ENVIO", "D302"));
            doclist.Add(new validdocs("COVE DETALLE", "D304"));
            doclist.Add(new validdocs("VU EDOCUMENT", "D303"));

            var filterdocs = new List<string>();
            filterdocs.Add("D001");
            filterdocs.Add("D004");
            filterdocs.Add("D010");
            filterdocs.Add("D006");
            filterdocs.Add("D023");
            filterdocs.Add("D018");
            filterdocs.Add("D022");
            filterdocs.Add("D103");
            filterdocs.Add("D102");
            filterdocs.Add("D027");
            filterdocs.Add("D104");
            filterdocs.Add("D105");
            filterdocs.Add("D201");
            filterdocs.Add("D202");
            filterdocs.Add("D205");
            filterdocs.Add("D203");
            filterdocs.Add("D204");
            filterdocs.Add("D011");
            filterdocs.Add("D012");
            filterdocs.Add("D301");
            filterdocs.Add("D302");
            filterdocs.Add("D304");
            filterdocs.Add("D303");

            MongoCollection<BsonDocument> _collection = _db.GetCollection<BsonDocument>("Documents");
            var query = Query.And(Query.In("key", BsonArray.Create(refs)), Query.In("doctype", BsonArray.Create(filterdocs)));

            var cursor = _collection.Find(query).SetFields(new[] { "key", "doctype" });
            var bsonlist = cursor.ToList<BsonDocument>();
            var fulldoclist = new List<validdocs>();
            foreach (BsonDocument bsd in bsonlist)
            {
                var mydoc = new validdocs();
                mydoc.docref = bsd["key"].AsBsonArray[0].ToString();
                mydoc.doctype = bsd["doctype"].ToString();
                fulldoclist.Add(mydoc);
            }

            return fulldoclist;
        }
        public class validdocs
        {
            public validdocs() { }
            public validdocs(string dcname, string did)
            {
                docid = did;
                docname = dcname;
            }
            public string docref { get; set; }
            public string docid { get; set; }
            public string doctype { get; set; }
            public string docname { get; set; }
        }
        public string GetSelectedintoZipCtasGastos(List<newdocument> docs, string txtref, string mailto, string encref, string txtsubject, bool mergefiles, bool compressfiles)
        {
            string archiveName = System.Web.HttpContext.Current.Server.MapPath("~/tempdigitalfiles/") + txtref + ".zip";
            string archiveNameMerge = System.Web.HttpContext.Current.Server.MapPath("~/tempdigitalfiles/") + txtref + "_full.pdf";
            string newFileNamex = string.Empty;

            var mergelist = new List<string>();
            var nopdflist = new List<string>();
            foreach (newdocument docfor in docs)
            {
                string filename = string.Empty;
                BsonValue myId_pdf = docfor.fileid;
                newFileNamex = System.Web.HttpContext.Current.Server.MapPath("~/tempdigitalfiles/") + docfor.savedFileName;
                if (!File.Exists(newFileNamex))
                {
                    var file = _db.GridFS.FindOne(Query.EQ("_id", myId_pdf));
                    using (var stream = file.OpenRead())
                    {
                        try
                        {
                            var bytes = new byte[stream.Length];
                            stream.Read(bytes, 0, (int)stream.Length);
                            using (var newFs = new System.IO.FileStream(newFileNamex, System.IO.FileMode.Create))
                            {
                                newFs.Write(bytes, 0, bytes.Length);
                            }
                            if (docfor.fileExtension == "pdf" && mergefiles)
                            {
                                if (IsValidPdf(newFileNamex))
                                    mergelist.Add(newFileNamex);
                            }
                            else
                            {
                                nopdflist.Add(newFileNamex);
                            }

                        }
                        catch { }
                    }
                }
                else
                {
                    if (docfor.fileExtension == "pdf" && mergefiles)
                    {
                        if (IsValidPdf(newFileNamex))
                            mergelist.Add(newFileNamex);
                    }
                    else
                    {
                        nopdflist.Add(newFileNamex);
                    }
                }
            }


            if (System.IO.File.Exists(archiveName))
                System.IO.File.Delete(archiveName);

            if (mergefiles)
                Merge(mergelist, archiveNameMerge);

            if (compressfiles)
            {
                if (mergelist.Count > 0 || nopdflist.Count > 0)
                {
                    using (ZipFile zip = new ZipFile())
                    {
                        if (mergelist.Count > 0)
                            zip.AddFile(archiveNameMerge, "Expediente");
                        foreach (var nopdf in nopdflist)
                        {
                            zip.AddFile(nopdf, "Expediente");
                        }
                        zip.Save(archiveName);
                        zip.Dispose();
                    }
                }
            }
            if (mailto != "")
            {
                core.EmailHandler newmail = new core.EmailHandler();
                var subject = "Consorcio Aduanero del Bajío - Expediente Digital : " + txtref;

                if (txtsubject != "")
                    subject = txtsubject;

                string boddy = "";

                boddy = string.Format(@"<div align='center'>
                    <table class='MsoNormalTable' border='0' cellspacing='0' cellpadding='0' width='600' style='width:450.0pt;border-collapse:collapse'>
                        <tbody>
                            <tr style='height:6.0pt'>
                                <td width='600' style='width:450.0pt;padding:0cm 0cm 0cm 0cm;height:6.0pt'></td>
                            </tr>
                            <tr style='height:6.0pt'>
                                <td width='600' style='width:450.0pt;padding:0cm 0cm 0cm 0cm;height:6.0pt'>
                                    <p class='MsoNormal'>A quien corresponda: Le informamos que&nbsp;Consorcio Aduanero del Bajío, S.C. ha puesto a su disposición la descarga de documentos relacionados con los servicios que le han ofrecido. Para ver dichos documentos abrir el archivo adjunto a este correo, si desea consultar el expediente completo favor de dar click en el sig. enlace: <o:p></o:p></p>
                                    <p class='MsoNormal' align='center' style='text-align:center'><a href='https://www.grupoproeci.com.mx/vdigital.aspx?key={0}'><br>
                                    Descargar </a><o:p></o:p></p>
                                    <p>Tenga presente que este mensaje es una notificación automática, no lo responda ya que esta cuenta no es monitoreada por nuestro departamento de Cobranza Consorcio Aduanero del Bajío, S.C.
                                    <o:p></o:p></p>
                                </td>
                            </tr>
                            <tr style='height:6.0pt'>
                                <td width='600' style='width:450.0pt;padding:0cm 0cm 0cm 0cm;height:6.0pt'>
                                    <p class='MsoNormal'>AVISO DE CONFIDENCIALIDAD: Este mensaje es confidencial y/o puede contener información privilegiada. Si usted no es el destinatario o no es alguna persona autorizada por éste para recibir el mensaje, usted no deberá utilizar, copiar, revelar
                                        o tomar ninguna acción basada en este mensaje o cualquier otra información incluida en él. Si recibe este mensaje por error, por favor notifíquelo de inmediato al remitente y bórrelo de su computadora.
                                    <o:p></o:p></p>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>", encref);

                newmail.trackcxc(mailto, subject, encref, archiveName, compressfiles, nopdflist);
            }

            foreach (newdocument docfor in docs)
            {
                string delFileNamex = System.Web.HttpContext.Current.Server.MapPath("~/tempdigitalfiles/") + docfor.savedFileName;
                try
                {
                    if (System.IO.File.Exists(delFileNamex))
                        System.IO.File.Delete(delFileNamex);
                }
                catch { }
            }

            return "";

        }
        public string GetFullintoZip(List<newdocument> docs, string txtref)
        {
            string archiveName = System.Web.HttpContext.Current.Server.MapPath("~/tempdigitalfiles/") + txtref + ".zip";
            string archiveNameMerge = System.Web.HttpContext.Current.Server.MapPath("~/tempdigitalfiles/") + txtref + "_full.pdf";
            string newFileNamex = string.Empty;

            var mergelist = new List<string>();
            var nopdflist = new List<string>();
            foreach (newdocument docfor in docs)
            {
                string filename = string.Empty;
                BsonValue myId_pdf = docfor.fileid;
                newFileNamex = System.Web.HttpContext.Current.Server.MapPath("~/tempdigitalfiles/") + docfor.savedFileName;
                if (!File.Exists(newFileNamex))
                {
                    var file = _db.GridFS.FindOne(Query.EQ("_id", myId_pdf));
                    using (var stream = file.OpenRead())
                    {
                        try
                        {
                            var bytes = new byte[stream.Length];
                            stream.Read(bytes, 0, (int)stream.Length);
                            using (var newFs = new System.IO.FileStream(newFileNamex, System.IO.FileMode.Create))
                            {
                                newFs.Write(bytes, 0, bytes.Length);
                            }
                            nopdflist.Add(newFileNamex);

                        }
                        catch { }
                    }
                }
                else
                {
                    nopdflist.Add(newFileNamex);
                }
            }


            if (System.IO.File.Exists(archiveName))
                System.IO.File.Delete(archiveName);

            using (ZipFile zip = new ZipFile())
            {
                foreach (var nopdf in nopdflist)
                {
                    zip.AddFile(nopdf, "Expediente");
                }
                zip.Save(archiveName);
                zip.Dispose();
            }
            foreach (newdocument docfor in docs)
            {
                string delFileNamex = System.Web.HttpContext.Current.Server.MapPath("~/tempdigitalfiles/") + docfor.savedFileName;
                try
                {
                    if (System.IO.File.Exists(delFileNamex))
                        System.IO.File.Delete(delFileNamex);
                }
                catch { }
            }

            return System.IO.Path.GetFileName(archiveName);

        }
        public string copyfullexp(string sourceref, string targetref)
        {
            BsonValue myId_pdf;
            MongoCollection<BsonDocument> _collection = _db.GetCollection<BsonDocument>("Documents");

            List<string> list = new List<string>();
            list.Add(sourceref);

            var sort = SortBy.Descending("doctype");
            var query = MongoDB.Driver.Builders.Query.In("key", BsonArray.Create(list));

            var cursor = _collection.Find(query);
            int i = 0;
            List<newdocument> generatedfiles = new List<newdocument>();
            IList<newProfile> doclist = new List<newProfile>();
            foreach (var sdoc in cursor)
            {
                myId_pdf = (BsonValue)sdoc["idfilename"];

                //var doctypename = GetCollDataFieldValue("DocTypes", "code", sdoc["doctype"].ToString(), "name"); ;

                newdocument newdoc = new newdocument();
                newdoc.fileid = myId_pdf;
                newdoc.reference = sdoc["key"].ToString();
                newdoc.savedFileName = sdoc["key"].ToString() + "_" + sdoc["idfilename"].ToString() + "." + sdoc["content_type"].ToString();
                newdoc.fileExtension = sdoc["content_type"].ToString();
                newdoc.comments = sdoc["comments"].ToString();
                newdoc.doctype = sdoc["doctype"].ToString();

                generatedfiles.Add(newdoc);
            }

            return _copyfullexp(generatedfiles, targetref);
        }
        private string _copyfullexp(List<newdocument> docs, string targetref)
        {
            string newFileNamex = string.Empty;

            foreach (newdocument docfor in docs)
            {
                string filename = string.Empty;
                BsonValue myId_pdf = docfor.fileid;
                newFileNamex = System.Web.HttpContext.Current.Server.MapPath("~/tempdigitalfiles/") + docfor.savedFileName;
                if (!File.Exists(newFileNamex))
                {
                    var file = _db.GridFS.FindOne(Query.EQ("_id", myId_pdf));
                    using (var stream = file.OpenRead())
                    {
                        try
                        {
                            var bytes = new byte[stream.Length];
                            stream.Read(bytes, 0, (int)stream.Length);
                            using (var newFs = new System.IO.FileStream(newFileNamex, System.IO.FileMode.Create))
                            {
                                newFs.Write(bytes, 0, bytes.Length);
                            }

                        }
                        catch { }
                    }
                }
            }

            foreach (newdocument docfor in docs)
            {
                string delFileNamex = System.Web.HttpContext.Current.Server.MapPath("~/tempdigitalfiles/") + docfor.savedFileName;
                string sourceFile = "~/tempdigitalfiles/" + docfor.savedFileName;

                SaveCopiedDocument(targetref, new BsonArray(), docfor.comments, "COPIA", sourceFile, docfor.doctype, true, true, "", "", docfor.fileExtension, "AUTO");

                try
                {
                    if (System.IO.File.Exists(delFileNamex))
                        System.IO.File.Delete(delFileNamex);
                }
                catch { }
            }

            return "";

        }
        public void SaveCopiedDocument(string sKey, BsonArray sKey2_extra, string sNotas, string sOrigin, string localfile, string tipodoc, bool clientaccess, bool expires, string datefrom, string dateto, string ext, string user)
        {

            MongoClient _client;
            MongoServer _server;
            MongoDatabase _db;

            string connectionstring = "mongodb://" + mongoHost + ":" + mongoPort;
            _client = new MongoClient(connectionstring);
            _server = _client.GetServer();
            _db = _server.GetDatabase("digital");

            InsertDataDocumentos(_db, sKey, sKey2_extra, sNotas, sOrigin, localfile, tipodoc, clientaccess, expires, datefrom, dateto, ext, user);

        }
        private bool IsValidPdf(string filepath)
        {
            bool Ret = true;
            PdfReader reader = null;
            try
            {
                reader = new PdfReader(filepath);
            }
            catch
            {
                Ret = false;
            }
            return Ret;
        }
        private void Merge(List<String> InFiles, String OutFile)
        {
            using (FileStream stream = new FileStream(OutFile, FileMode.Create))
            using (Document doc = new Document())
            using (PdfCopy pdf = new PdfCopy(doc, stream))
            {
                doc.Open();
                PdfReader reader = null;
                PdfImportedPage page = null;
                //fixed typo
                InFiles.ForEach(file =>
                {
                    reader = new PdfReader(file);
                    for (int i = 0; i < reader.NumberOfPages; i++)
                    {
                        page = pdf.GetImportedPage(reader, i + 1);
                        pdf.AddPage(page);
                    }
                    pdf.FreeReader(reader);
                    reader.Close();
                });
            }
        }

        public BsonArray GetDataSinAduana(string sRef, string sKey2)
        {
            string sKeyExtra = String.Empty;
            string sPref = String.Empty;   // AMCC Aqui poner el pref de la sucursal
            BsonArray originalTags = new BsonArray();

            originalTags = new BsonArray { 
                    "d_" + sKey2, 
                    "pref_" + ( String.IsNullOrEmpty(sPref) ? "" : sPref.ToString().Trim() )
                 };

            return originalTags;
        }

        public class newProfile
        {
            public string Id { get; set; }
            public string key { get; set; }
            public string key2 { get; set; }
            public string content_type { get; set; }
            public string idfilename { get; set; }
            public string filename { get; set; }
            public string fullfilename { get; set; }
            public double lenght { get; set; }
            public string uploaddate { get; set; }
            public string uploaddatenew { get; set; }
            public string doctype { get; set; }
            public string doctypename { get; set; }
            public string expdate { get; set; }
            public string docdate { get; set; }
            public string docdatenew { get; set; }
            public string tags { get; set; }
            public int year { get; set; }
            public int month { get; set; }
            public int day { get; set; }
            public string source { get; set; }
            public string comments { get; set; }
            public string user { get; set; }
            public bool isCompleted { get; set; }
            public string controlid { get; set; }
        }

        public class newdocument
        {
            public BsonValue fileid { get; set; }
            public string reference { get; set; }
            public bool fileexpire { get; set; }
            public string datefrom { get; set; }
            public string dateto { get; set; }
            public bool clientaccess { get; set; }
            public string doctype { get; set; }
            public string comments { get; set; }
            public string fileExtension { get; set; }
            public string guidname { get; set; }
            public string savedFileName { get; set; }
            public string user { get; set; }
            public IList<newdocument> Doc { get; set; }
        }
        public class _responsedocument
        {
            public string Referencia { get; set; }
            public bool Expira { get; set; }
            public string FechaInicio { get; set; }
            public string FechaFinal { get; set; }
            public bool AccesoClientes { get; set; }
            public string Tipo_Documento { get; set; }
            public string Commentarios { get; set; }
            public string Extencion { get; set; }
            public IList<_responsedocument> _doc { get; set; }
        }

    }
}
