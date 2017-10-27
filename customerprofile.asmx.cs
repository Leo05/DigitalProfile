using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using System.Data;

namespace SMIWeb.services
{
    /// <summary>
    /// Summary description for customerprofile
    /// </summary>
    [WebService(Namespace = "http://www.proeci.com.mx/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class customerprofile : System.Web.Services.WebService
    {
        [WebMethod(Description = "Load Customers", EnableSession = true)]
        [System.Web.Script.Services.ScriptMethod(ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public string repcustomerscomp()
        {
            string tsql = string.Format(@"smidashboard_sps_depuracionclientes");
            core.extendsql sqlService = new core.extendsql();
            sqlService.cmdType = core.extendsql.CommandTypes.SP;
            sqlService.cnnName = core.extendsql.ConnectionNames.def;
            sqlService.sqltxt = tsql;
            DataTable ds = new DataTable();
            ds = sqlService.GetAsyncDataTableMethod();
            string jsonText = JsonConvert.SerializeObject(sqlService.GetAsyncDataTableMethod(), Newtonsoft.Json.Formatting.None);
            return jsonText;
        }
        [WebMethod(Description = "", EnableSession = true)]
        [System.Web.Script.Services.ScriptMethod(ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public string loadtarifa(string frac)
        {
            string jsonText = "[{\"Rows\": 0}]";
            core.extendsql sqlService = new core.extendsql();
            sqlService.cnnName = core.extendsql.ConnectionNames.local;
            sqlService.cmdType = core.extendsql.CommandTypes.SP;
            sqlService.sqltxt = "[smidashboard_sps_gettarifa]";
            sqlService.RequestParameters.Add(new core.extendsql.rspt("@FRACCION", frac.Replace(".", "")));
            jsonText = JsonConvert.SerializeObject(sqlService.GetAsyncDataTableMethod(), Newtonsoft.Json.Formatting.None);
            System.Web.HttpContext.Current.Response.ContentType = "application/json";
            return jsonText.Replace("@", "");
        }
        [WebMethod(Description = "Load Customer by ID", EnableSession = true)]
        [System.Web.Script.Services.ScriptMethod(ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public string getCustomerbyID(int id)
        {
            string jsonText = "[{\"Rows\": 0}]";
            core.extendsql sqlService = new core.extendsql();
            sqlService.cmdType = core.extendsql.CommandTypes.SP;
            sqlService.cnnName = core.extendsql.ConnectionNames.explegal;
            sqlService.sqltxt = "SMIDASHBOARD_SPS_GETPERFILCTE";
            sqlService.RequestParameters.Add(new core.extendsql.rspt("@CTEID", id));
            jsonText = JsonConvert.SerializeObject(sqlService.GetAsyncDataSetMethod(), Newtonsoft.Json.Formatting.None);
            System.Web.HttpContext.Current.Response.ContentType = "application/json";
            return jsonText;
        }
        [WebMethod(Description = "Load Contacts List", EnableSession = true)]
        [System.Web.Script.Services.ScriptMethod(ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public string loadContacts(int cust_id)
        {
            string jsonText = "[{\"Rows\": 0}]";
            core.extendsql sqlService = new core.extendsql();
            sqlService.cmdType = core.extendsql.CommandTypes.TXT;
            sqlService.cnnName = core.extendsql.ConnectionNames.explegal;
            sqlService.sqltxt = string.Format(@"SELECT contclaveunica,
	                                                    contnumero,
	                                                    contnombre,
	                                                    contapellido,
	                                                    contfkpuesto,
	                                                    contemail,
	                                                    contrfc,
	                                                    conttelefono,
	                                                    contfktipoop,
	                                                    contfkaduana,
	                                                    contprincipal,
	                                                    contreplegal,
	                                                    contfechacreacion,
	                                                    contfkuidcreacion from CLIENTES_CONTACTOS where contfkcteid={0}", cust_id);
            jsonText = JsonConvert.SerializeObject(sqlService.GetAsyncDataTableMethod(), Newtonsoft.Json.Formatting.None);
            System.Web.HttpContext.Current.Response.ContentType = "application/json";
            return jsonText;
        }
        [WebMethod(Description = "Load ProfileB List", EnableSession = true)]
        [System.Web.Script.Services.ScriptMethod(ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public string addnewcontact(string contnumero, string contnombre, string contapellido, string contfkpuesto, string contemail, string contrfc, string conttelefono,
                                  string contfktipoop, string contfkaduana, string contprincipal, string contreplegal)
        {
            string jsonText = "[{\"Rows\": 0}]";
            core.extendsql sqlService = new core.extendsql();
            sqlService.cmdType = core.extendsql.CommandTypes.TXT;
            sqlService.cnnName = core.extendsql.ConnectionNames.explegal;
            sqlService.sqltxt = string.Format(@"INSERT INTO CLIENTES_CONTACTOS(contnumero,contnombre,contapellido,contfkpuesto,contemail,contrfc,conttelefono,contfktipoop,contfkaduana,contreplegal) 
                                                            VALUES ('{0}','{1}','{2}',{3},'{4}','{5}','{6}',{7},{8},{9});
                                                SELECT contclaveunica,
	                                                    contnumero,
	                                                    contnombre,
	                                                    contapellido,
	                                                    contfkpuesto,
	                                                    contemail,
	                                                    contrfc,
	                                                    conttelefono,
	                                                    contfktipoop,
	                                                    contfkaduana,
	                                                    contprincipal,
	                                                    contreplegal,
	                                                    contfechacreacion,
	                                                    contfkuidcreacion FROM CLIENTES_CONTACTOS WHERE contfkcteid={15} ORDER BY contnombre;",
                                               contnumero, contnombre, contapellido, contfkpuesto, contemail, contrfc, conttelefono, contfktipoop, contfkaduana, contprincipal, contreplegal);
            jsonText = JsonConvert.SerializeObject(sqlService.GetAsyncDataTableMethod(), Newtonsoft.Json.Formatting.None);
            System.Web.HttpContext.Current.Response.ContentType = "application/json";
            return jsonText;
        }
        [WebMethod(Description = "Load XXX List", EnableSession = true)]
        [System.Web.Script.Services.ScriptMethod(ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public string editcontact(string contnumero, string contnombre, string contapellido, string contfkpuesto, string contemail, string contrfc, string conttelefono,
                                  string contfktipoop, string contfkaduana, string contprincipal, string contreplegal)
        {
            string jsonText = "[{\"Rows\": 0}]";
            core.extendsql sqlService = new core.extendsql();
            sqlService.cmdType = core.extendsql.CommandTypes.TXT;
            sqlService.cnnName = core.extendsql.ConnectionNames.explegal;
            sqlService.sqltxt = string.Format(@"UPDATE CLIENTES_CONTACTOS
                                               SET [contnombre] = '{0}'
                                                  ,[contapellido] = '{1}'
                                                  ,[contfkpuesto] = {2}
                                                  ,[contemail] = '{3}'
                                                  ,[contrfc] = '{4}'
                                                  ,[conttelefono] = '{5}'
                                                  ,[contfktipoop] = {6}
                                                  ,[contfkaduana] = {7}
                                                  ,[contprincipal] = {8}
                                                  ,[contreplegal] = {9}
                                             WHERE contid={14};
                                             SELECT contclaveunica,
	                                                    contnumero,
	                                                    contnombre,
	                                                    contapellido,
	                                                    contfkpuesto,
	                                                    contemail,
	                                                    contrfc,
	                                                    conttelefono,
	                                                    contfktipoop,
	                                                    contfkaduana,
	                                                    contprincipal,
	                                                    contreplegal,
	                                                    contfechacreacion,
	                                                    contfkuidcreacion FROM CLIENTES_CONTACTOS WHERE contfkcteid={15} ORDER BY contnombre;",
                                                        contnumero, contnombre, contapellido, contfkpuesto, contemail, contrfc, conttelefono, contfktipoop, contfkaduana, contprincipal, contreplegal);
            jsonText = JsonConvert.SerializeObject(sqlService.GetAsyncDataTableMethod(), Newtonsoft.Json.Formatting.None);
            System.Web.HttpContext.Current.Response.ContentType = "application/json";
            return jsonText;
        }
    }
}
