(function (namespace, $) {
    "use strict";

    var expedientedigital = function () {
        // Create reference to this instance
        var o = this;
        // Initialize app when document is ready
        $(document).ready(function () {
            o.initialize();
        });

    };
    var p = expedientedigital.prototype;
    var theme = 'arctic';
    var drop;

    p.initialize = function () {
        this._initUser();
        this._initializeDom();
        this._getDocList();
    };
    p._initUser = function () {
        var date = new Date(Date());
        var options = {
            weekday: "long", year: "numeric", month: "short",
            day: "numeric"
        };
    };

    p._initializeDom = function () {
        $("#loading").hide();
        $("#jqxToolBar").jqxToolBar({
            width: '100%', height: 40, theme: theme, tools: 'custom custom custom custom custom custom custom custom',
            initTools: function (type, index, tool, menuToolIninitialization) {
                switch (index) {
                    case 0:
                        var input = $("<div id='inputref'><input type='text' /><div id='searchref'><i class='md md-search'></i></div></div>");
                        tool.append(input);
                        input.jqxInput({ placeHolder: "Referencia", theme: theme, width: 280, height: 28 });
                        break;
                    case 1:
                        var button = $("<div id='cmduploadfile'>" + "<span class='md md-cloud-upload'></span>" + "  Subir archivos</div>");
                        button.attr("title", "Subir Archivos");
                        button.data("toggle", "modal");
                        button.data("target", "formfileupload");
                        tool.append(button);
                        button.jqxButton({ theme: theme, width: 120, height: 20 });
                        break;
                    case 2:
                        var button = $("<div id='cmdselectfiles'>" + "<span class='md md-check-box'></span>" + "  Seleccionar</div>");
                        tool.append(button);
                        button.jqxToggleButton({ toggled: false, theme: theme, width: 120, height: 20 });
                        break;
                    case 3:
                        var button = $("<div id='cmddownfiles'>" + "<span class='md md-cloud-download'></span>" + "  Descargar</div>");
                        button.attr("title", "Descargar archivos selecionados");
                        tool.append(button);
                        button.jqxButton({ theme: theme, width: 120, height: 20 });
                        break;
                    case 4:
                        var button = $("<div id='cmdenviar'>" + "<span class='md md-email'></span>" + "  EMail</div>");
                        button.attr("title", "Enviar archivos por email");
                        tool.append(button);
                        button.jqxButton({ theme: theme, width: 120, height: 20 });
                        break;
                    case 5:
                        var button = $("<div id='cmdcopiaexp'>" + "<span class='md md-description'></span>" + "  Copiar Expediente</div>");
                        button.attr("title", "Copiar los archivos de una referencia a otra");
                        tool.append(button);
                        button.jqxButton({ theme: theme, width: 140, height: 20 });
                        break;
                    case 6:
                        var button = $("<div id='cmdnotcuenta'>" + "<span class='md md-description'></span>" + "  Notificar Cuenta</div>");
                        button.attr("title", "Notificar cuenta de gastos a cliente por correo electronico, en el correo se incluiran archivos adjuntos como pedimento y cuenta de gastos, los contactos se toman del catalogo de SICA");
                        button.data("toggle", "modal");
                        button.data("target", "formctagastos");
                        tool.append(button);
                        button.jqxButton({ theme: theme, width: 140, height: 20 });
                        break
                    case 7:
                        var button = $("<div id='cmdnotcuentacomp'>" + "<span class='md md-description'></span>" + "  Notificar Cuenta Complementaria</div>");
                        button.attr("title", "Notificar cuenta de gastos a cliente por correo electronico, en el correo se incluiran archivos adjuntos como pedimento y cuenta de gastos, los contactos se toman del catalogo de SICA");
                        button.data("toggle", "modal");
                        button.data("target", "formctagastos");
                        tool.append(button);
                        button.jqxButton({ theme: theme, width: 250, height: 20 });
                        break;
                }
            }
        });

        $("#tabexpo").click(function () {
            p._loadGridTable();
        });
        $("#cmdnotcuenta").click(function () {
            p._getLoadContactosSica();
        });
        $("#searchref").click(function () {
            var value = $("#inputref").val();
            p._loadByRef();
        });
        $('#inputref').bind('keyup', function (e) {
            if (e.keyCode === 13) { // 13 is enter key
                var value = $("#inputref").val();
                p._loadByRef();
            }
        });
        $("#cmdselectfiles").click(function () {
            var toggled = $("#cmdselectfiles").jqxToggleButton("toggled");
            if (toggled) {
                $("input[type='checkbox']").each(function () {
                    this.checked = true;
                });
            } else {
                $("input[type='checkbox']").each(function () {
                    this.checked = !this.checked;
                });
            }
        });

        $("#cmddownfiles").click(function () {
            p._downloadSelected();
        });
        $("#cmduploadfile").click(function () {
            var ref = $("#ctrlref").val();
            if (ref === "") {
                $("#warningmsg").text("No ha abierto ningun expediente !!");
                $("#warningnotification").jqxNotification("open");
                return;
            }
            $('#formfileupload').modal('show');
        });

        $("#btnenviarcomments").click(function () {
            p._upploadfilesbyref();
        });

        $("#cmdcopiaexp").click(function () {
            bootbox.prompt("Teclee referencia donde se copiaran los archivos", function (result) {
                if (result != null) {
                    p._copyprofile(result);
                }
            });
        });

        $('#formctagastos').on('hidden.bs.modal', function (e) {
            $('#textarea1').val('');
        });
        $("#btnenviarctagastos").click(function () {
            bootbox.confirm("Notificar cuenta de gastos a cliente?",
                function (result) {
                    if (result == true)
                        p._sendctaaviso();
                });
        });

        $('#emailWindow').jqxWindow({
            width: 400,
            height: 110, resizable: false, isModal: true, theme: theme, autoOpen: false,
            cancelButton: $('#cancelButton'),
            initContent: function () {
                $('#searchTextButton').jqxButton({ width: '80px', disabled: false, theme: theme });
                $('#cancelButton').jqxButton({ width: '80px', disabled: false, theme: theme });
                $('#matchCaseCheckBox').jqxCheckBox({ width: '250px', theme: theme });
                $('#emailto').jqxInput({ width: 200, height: 25, minLength: 1, theme: theme });
            }
        });
        $('#window').jqxWindow({
            showCollapseButton: true, height: 590, width: 999,
            theme: theme, maxWidth: 1000, maxHeight: 1000,
            resizable: true, isModal: true, autoOpen: false
        });
        $('#mainSplitter').jqxSplitter({ width: 990, height: 500, panels: [{ size: 300 }, { size: 700 }] });
        $('#deletewindow').jqxWindow({
            height: 200, width: 300,
            resizable: false, isModal: true, modalOpacity: 0.3,
            okButton: $('#cmddelete'), cancelButton: $('#cmdcanceldelete'), theme: theme, autoOpen: false,
            initContent: function () {
                $("#jqxPasswordInput").jqxPasswordInput({
                    width: 250, height: 25, placeHolder: "Teclear password", showStrength: false, theme: theme
                });
                $('#cmddelete').jqxButton({ width: 100, theme: theme });
                $('#cmdcanceldelete').jqxButton({ width: 100, theme: theme });
                $('#cmdcanceldelete').focus();
            }
        });
        $('#cmddelete').click(function () {
            p._deleteSinglefile();
        });
        $(document).on('contextmenu', function (e) {
            return false;
        });
        $("#cmdenviar").click(function () {
            $('#emailWindow').jqxWindow();
            $('#emailWindow').jqxWindow('open');
            var email = $.cookie('uemail');
            $('#emailto').val(email);
            $('#emailWindow').jqxWindow('open');
        });
        $('#searchTextButton').click(function () {
            p._sendSelected();
        });
        $('#contentexpander').jqxExpander({ width: '100%', theme: theme });


        $("#messageNotification").jqxNotification({
            width: 250, position: "top-right", opacity: 0.9,
            autoOpen: false, animationOpenDelay: 800, autoClose: true, autoCloseDelay: 5000, template: "info", theme: theme
        });
        $("#errorTimeOutNotification").jqxNotification({ width: 250, position: "top-right", autoCloseDelay: 5000, autoOpen: false, closeOnClick: true, autoClose: true, template: "error", animationOpenDelay: 800 });
        $("#warningnotification").jqxNotification({
            width: 250, position: "top-right", opacity: 0.9,
            autoOpen: false, animationOpenDelay: 800, autoClose: true, autoCloseDelay: 5000, template: "warning", theme: theme
        });
        // create jqxcalendar.
        $("#jqxwidgetdatetime").jqxDateTimeInput({ width: 230, height: 25, selectionMode: 'range', theme: theme });
        // create textarea.
        var quotes = [];
        quotes.push('IMAGEN DE MERCANCIA');
        quotes.push('ENTRADA A BODEGA');
        quotes.push('IMAGEN DE SELLO FISCAL');
        quotes.push('BILL OR LADING');
        quotes.push('FACTURA COMERCIAL');
        $('#jqxTextArea').jqxTextArea({ placeHolder: 'Comentarios...', height: 90, width: '100%', minLength: 1, source: quotes, theme: theme });

        var validformats = ".pdf,.doc,.docx,.odt,.xls,.xlsx,.xml,.jpg,.jpeg,.png,.ppt,.gif";
        Dropzone.autoDiscover = false;
        $("#mydropzone").dropzone({
            url: "services/FileHandler.ashx",
            addRemoveLinks: true,
            acceptedFiles: validformats,
            sending: function (file, xhr, formData) {
                var ref = $("#inputref").val().toUpperCase();
                if (ref == "") {
                    $("#warningmsg").text("No ha tecleado referencia !!");
                    $("#warningnotification").jqxNotification("open");
                    this.removeFile(file);
                }
                formData.append('ref', ref);

                if ($('#dateexp').is(":checked")) {
                    var selection = $("#jqxwidgetdatetime").jqxDateTimeInput('getRange');
                    if (selection.from == null) {
                        $("#warningmsg").text("Atencion !! ha seleccionado aplicar vigencia, pero aun no selecciona rango de fechas");
                        $("#warningnotification").jqxNotification("open");
                        this.removeFile(file);
                    }
                    formData.append('fileexpire', $('#dateexp').is(":checked"));
                    formData.append('datefrom', selection.from.toLocaleDateString());
                    formData.append('dateto', selection.to.toLocaleDateString());
                }
                else {
                    formData.append('fileexpire', false);
                    formData.append('datefrom', "");
                    formData.append('dateto', "");
                }

                formData.append('clientaccess', $('#clientaccess').is(":checked"));
                var comments = $("#jqxTextArea").val();
                formData.append('comments', comments);

                formData.append('doctype', $("#selectTipo option:selected").val());
                formData.append('tipodoc', $("#selectTipo option:selected").text());

            },
            success: function (file, response) {
                var imgName = response;
                //file.previewElement.classList.add("dz-success");
                var node, _i, _len, _ref, _results;
                var message = response // modify it to your error message
                file.previewElement.classList.add("dz-error");
                _ref = file.previewElement.querySelectorAll("[data-dz-errormessage]");
                _results = [];
                for (_i = 0, _len = _ref.length; _i < _len; _i++) {
                    node = _ref[_i];
                    _results.push(node.textContent = message);
                }
                console.log("Successfully uploaded :" + imgName);
            },
            removedfile: function (file) {
                var _ref; // Remove file on clicking the 'Remove file' button
                return (_ref = file.previewElement) != null ? _ref.parentNode.removeChild(file.previewElement) : void 0;
            }, error: function (xhr, textStatus, error) {
                var errorM = $.parseJSON(xhr.responseText);
                $("#errormsg").text(errorM.Message);
                $("#errorTimeOutNotification").jqxNotification("open");
            }
        });

        $("#table").jqxDataTable(
        {
            width: "150px",
            editable: true,
            showToolbar: true,
            altRows: true,
            height: '350px',
            ready: function () {
                // called when the DataTable is loaded.         
            },
            toolbarHeight: 35,
            renderToolbar: function (toolBar) {
                var toTheme = function (className) {
                    if (theme == "") return className;
                    return className + " " + className + "-" + theme;
                }
                // appends buttons to the status bar.
                var container = $("<div style='overflow: hidden; position: relative; height: 100%; width: 100%;'></div>");
                var buttonTemplate = "<div style='float: left; padding: 3px; margin: 2px;'><div style='margin: 4px; width: 16px; height: 16px;'></div></div>";
                var addButton = $(buttonTemplate);
                var editButton = $(buttonTemplate);
                var deleteButton = $(buttonTemplate);
                var cancelButton = $(buttonTemplate);
                var updateButton = $(buttonTemplate);
                container.append(addButton);
                container.append(editButton);
                container.append(deleteButton);
                container.append(cancelButton);
                container.append(updateButton);
                toolBar.append(container);
                addButton.jqxButton({ cursor: "pointer", enableDefault: false, height: 25, width: 25 });
                addButton.find('div:first').addClass(toTheme('jqx-icon-plus'));
                addButton.jqxTooltip({ position: 'bottom', content: "Add" });
                editButton.jqxButton({ cursor: "pointer", disabled: true, enableDefault: false, height: 25, width: 25 });
                editButton.find('div:first').addClass(toTheme('jqx-icon-edit'));
                editButton.jqxTooltip({ position: 'bottom', content: "Edit" });
                deleteButton.jqxButton({ cursor: "pointer", disabled: true, enableDefault: false, height: 25, width: 25 });
                deleteButton.find('div:first').addClass(toTheme('jqx-icon-delete'));
                deleteButton.jqxTooltip({ position: 'bottom', content: "Delete" });
                updateButton.jqxButton({ cursor: "pointer", disabled: true, enableDefault: false, height: 25, width: 25 });
                updateButton.find('div:first').addClass(toTheme('jqx-icon-save'));
                updateButton.jqxTooltip({ position: 'bottom', content: "Save Changes" });
                cancelButton.jqxButton({ cursor: "pointer", disabled: true, enableDefault: false, height: 25, width: 25 });
                cancelButton.find('div:first').addClass(toTheme('jqx-icon-cancel'));
                cancelButton.jqxTooltip({ position: 'bottom', content: "Cancel" });
                var updateButtons = function (action) {
                    switch (action) {
                        case "Select":
                            addButton.jqxButton({ disabled: false });
                            deleteButton.jqxButton({ disabled: false });
                            editButton.jqxButton({ disabled: false });
                            cancelButton.jqxButton({ disabled: true });
                            updateButton.jqxButton({ disabled: true });
                            break;
                        case "Unselect":
                            addButton.jqxButton({ disabled: false });
                            deleteButton.jqxButton({ disabled: true });
                            editButton.jqxButton({ disabled: true });
                            cancelButton.jqxButton({ disabled: true });
                            updateButton.jqxButton({ disabled: true });
                            break;
                        case "Edit":
                            addButton.jqxButton({ disabled: true });
                            deleteButton.jqxButton({ disabled: true });
                            editButton.jqxButton({ disabled: true });
                            cancelButton.jqxButton({ disabled: false });
                            updateButton.jqxButton({ disabled: false });
                            break;
                        case "End Edit":
                            addButton.jqxButton({ disabled: false });
                            deleteButton.jqxButton({ disabled: false });
                            editButton.jqxButton({ disabled: false });
                            cancelButton.jqxButton({ disabled: true });
                            updateButton.jqxButton({ disabled: true });
                            break;
                    }
                }
                var rowIndex = null;
                $("#table").on('rowSelect', function (event) {
                    var args = event.args;
                    rowIndex = args.index;
                    updateButtons('Select');
                });
                $("#table").on('rowUnselect', function (event) {
                    updateButtons('Unselect');
                });
                $("#table").on('rowEndEdit', function (event) {
                    updateButtons('End Edit');
                });
                $("#table").on('rowBeginEdit', function (event) {
                    updateButtons('Edit');
                });
                addButton.click(function (event) {
                    if (!addButton.jqxButton('disabled')) {
                        // add new empty row.
                        $("#table").jqxDataTable('addRow', null, { 'Email': 'EMail' }, 'first');
                        // select the first row and clear the selection.
                        $("#table").jqxDataTable('clearSelection');
                        $("#table").jqxDataTable('selectRow', 0);
                        // edit the new row.
                        $("#table").jqxDataTable('beginRowEdit', 0);
                        updateButtons('add');
                    }
                });
                cancelButton.click(function (event) {
                    if (!cancelButton.jqxButton('disabled')) {
                        // cancel changes.
                        $("#table").jqxDataTable('endRowEdit', rowIndex, true);
                    }
                });
                updateButton.click(function (event) {
                    if (!updateButton.jqxButton('disabled')) {
                        // save changes.
                        $("#table").jqxDataTable('endRowEdit', rowIndex, false);
                    }
                });
                editButton.click(function () {
                    if (!editButton.jqxButton('disabled')) {
                        $("#table").jqxDataTable('beginRowEdit', rowIndex);
                        updateButtons('edit');
                    }
                });
                deleteButton.click(function () {
                    if (!deleteButton.jqxButton('disabled')) {
                        $("#table").jqxDataTable('deleteRow', rowIndex);
                        updateButtons('delete');
                    }
                });
            },
            columns: [
              { text: 'Contacto', editable: false, dataField: 'Nombre', width: 150 },
              {
                  text: 'EMail ', editable: true, dataField: 'Email',
                  validation: function (cell, value) {
                      if (!p._isValidEmail(value)) return { message: "Debe ingresar una direccion de correo valida", result: false };
                      return true;
                  }
              }
            ]
        });
    };
    p._isValidEmail = function (email) {
        var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        return regex.test(email);
    }
    p._getChangeStatus = function (estatusid) {

        var ref = $("#inputref").val().toUpperCase();
        if (ref == "") {
            $("#warningmsg").text("No ha tecleado referencia !!");
            $("#warningnotification").jqxNotification("open");
            return;
        }
        var params = {
            txtref: ref
        }
        var url;
        switch (estatusid) {
            case 0:
                url = 'services/cambiaestatus.asmx/updateiniciorevision';
                break;
            case 1:
                url = "services/cambiaestatus.asmx/updatefinrevision";
                break;
            case 2:
                url = "services/cambiaestatus.asmx/updateiniciacarga";
                break;
            case 3:
                url = "services/cambiaestatus.asmx/updatefinrevision";
                break;
            case 4:
                url = "services/cambiaestatus.asmx/notificacuenta";
                break;
            default:
                url = ""
                break;
        }

        if (url == "")
            return;

        $.ajax({
            type: 'POST',
            data: JSON.stringify(params),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: true,
            url: url,
            success: function (data) {
                $("#successmsg").text("cambio realizado exitosamente!!");
                $("#messageNotification").jqxNotification("open");
            }, error: function (xhr, textStatus, error) {
                var errorM = $.parseJSON(xhr.responseText);
                $("#errormsg").text(errorM.Message);
                $("#errorTimeOutNotification").jqxNotification("open");
            }, complete: function () {
                dialog.modal('hide');
            }
        });
    };

    p._sendctaaviso = function () {

        var ref = $("#inputref").val().toUpperCase();
        var facturas = $('#ctrlfacturas').val();
        var consolidados = $('#ctrlconsolidados').val();
        var preentrada = $('#ctrlpreentrada').val();

        if (ref == "") {
            $("#warningmsg").text("No ha tecleado referencia !!");
            $("#warningnotification").jqxNotification("open");
            return;
        }

        var cnts = "";
        var rows = $("#table").jqxDataTable('getRows');
        for (var i = 0; i < rows.length; i++) {
            // get a row.
            var rowData = rows[i];
            if (p._isValidEmail(rowData.Email))
                cnts += rowData.Email + ";";
        }
        var txtsubject = $('#txtsubject').val();
        var mergefiles = $('#cbcxcmergefiles').prop('checked');
        var compressfiles = $('#cbcxccompressfiles').prop('checked');
        var archivo = $("#cuentaarchivo").text();

        var params = {
            tocontacts: cnts,
            txtref: ref,
            txtsubject: txtsubject,
            mergefiles: mergefiles,
            compressfiles: compressfiles,
            archivo: archivo,
            facturas: facturas,
            consolidados: consolidados,
            preentrada: preentrada
        }

        $('#formctagastos').modal('hide');
        var dialog = bootbox.dialog({
            message: '<p><i class="fa fa-spin fa-spinner"></i> por favor espere...</p>',
            size: 'small',
            closeButton: false
        });

        $.ajax({
            type: 'POST',
            data: JSON.stringify(params),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: true,
            url: 'services/cambiaestatus.asmx/envianotificacion',
            success: function (data) {
                $("#successmsg").text("envio realizado exitosamente!!");
                $("#messageNotification").jqxNotification("open");
            }, error: function (xhr, textStatus, error) {
                var errorM = $.parseJSON(xhr.responseText);
                $("#errormsg").text(errorM.Message);
                $("#errorTimeOutNotification").jqxNotification("open");
            }, complete: function () {
                dialog.modal('hide');
            }
        });
    };
    p._getJsonValue = function (key) {
        for (var i = 0; i < jsongrid.length; i++) {
            if (jsongrid[i].doctype == key) {
                return true;
            }
        }
        return false;
    };
    p._getLoadContactosSica = function () {

        var isComplete = false;

        var ref = $("#inputref").val().toUpperCase();
        if (ref == "") {
            $("#warningmsg").text("No ha tecleado referencia !!");
            $("#warningnotification").jqxNotification("open");
            return;
        }

        //Se elimino pedimento anterior D205 y D206
        //if (!(p._getJsonValue("D011") && p._getJsonValue("D012") && p._getJsonValue("D201") && p._getJsonValue("D202") && (p._getJsonValue("D205") || p._getJsonValue("D206")))) {

        if (ref.startsWith("E")) {
            if (!(p._getJsonValue("D012") && p._getJsonValue("D201") && p._getJsonValue("D202"))) {
                $("#errormsg").text("La referencia aun no cuenta con los documentos minimos para notificar la cuenta... favor de verificar !!");
                $("#errorTimeOutNotification").jqxNotification("open");
                return;
            }
        }
        else {
            if (!(p._getJsonValue("D011") && p._getJsonValue("D012") && p._getJsonValue("D201") && p._getJsonValue("D202"))) {
                $("#errormsg").text("La referencia aun no cuenta con los documentos minimos para notificar la cuenta... favor de verificar !!");
                $("#errorTimeOutNotification").jqxNotification("open");
                return;
            }
        }

        var params = {
            txtref: ref
        }
        var dialog = bootbox.dialog({
            message: '<p><i class="fa fa-spin fa-spinner"></i> por favor espere...</p>',
            size: 'small',
            closeButton: false
        });

        $.ajax({
            type: 'POST',
            data: JSON.stringify(params),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: true,
            url: 'services/cambiaestatus.asmx/traecontactossica',
            success: function (data) {

                $('#formctagastos').modal('show');
                var json = $.parseJSON(data.d);
                console.log(json);


                if (json.Table[0]) {
                    $("#txtctefac").text(json.Table[0].ClienteFactura);
                    $("#cuentaarchivo").text(json.Table[0].Archivo);
                }


                var ordersSource =
                {
                    localdata: json,
                    dataFields: [
                        { name: 'Nombre', type: 'string' },
                        { name: 'Email', type: 'string' }
                    ],
                    dataType: "json",
                    addRow: function (rowID, rowData, position, commit) {
                        commit(true);
                    },
                    updateRow: function (rowID, rowData, commit) {
                        commit(true);
                    },
                    deleteRow: function (rowID, commit) {
                        commit(true);
                    }
                };
                var dataAdapter = new $.jqx.dataAdapter(ordersSource, {
                    loadComplete: function () {
                    }
                });
                $("#table").jqxDataTable({ source: dataAdapter, width: '100%' });


                if (json.Table1[0] != undefined) {
                    bootbox.alert({
                        message: "Atencion!! esta cuenta ya fue notificada al cliente",
                        size: 'small'
                    });
                }
                var numped = $("#txtnumped").val();
                var ref = $("#txtref").val();

                $("#txtsubject").val("Pedimento: " + numped + " - Referencia: " + ref);

                $("#successmsg").text("cambro realizado exitosamente!!");
                $("#messageNotification").jqxNotification("open");
            }, error: function (xhr, textStatus, error) {
                var errorM = $.parseJSON(xhr.responseText);
                $("#errormsg").text(errorM.Message);
                $("#errorTimeOutNotification").jqxNotification("open");
            }, complete: function () {
                dialog.modal('hide');
            }
        });
    };
    p._getDocList = function () {
        var dialog = bootbox.dialog({
            message: '<p><i class="fa fa-spin fa-spinner"></i> por favor espere...</p>',
            size: 'small',
            closeButton: false
        });

        $.ajax({
            type: 'POST',
            data: JSON.stringify({}),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: true,
            url: 'services/expedientedigital.asmx/getdocumentlist',
            success: function (data) {
                $("#selectTipo").empty();

                var json = $.parseJSON(data.d);
                $.each(json, function (key, value) {
                    $("#selectTipo").append($('<option>').text(value.docname).attr('value', value.doccode));
                });

            }, error: function (xhr, textStatus, error) {
                var errorM = $.parseJSON(xhr.responseText);
                $("#errormsg").text(errorM.Message);
                $("#errorTimeOutNotification").jqxNotification("open");
            }, complete: function () {
                dialog.modal('hide');
            }
        });
    };
    p._isRightClick = function (event) {
        var rightclick;
        if (!event) var event = window.event;
        if (event.which) rightclick = (event.which == 3);
        else if (event.button) rightclick = (event.button == 2);
        return rightclick;
    };
    p._loadByRef = function () {
        var ref = $("#inputref").val().toUpperCase();
        if (ref == "") {
            $("#warningmsg").text("No ha tecleado referencia !!");
            $("#warningnotification").jqxNotification("open");
            return;
        }
        var params = {
            refby: ref,
            encref: '',
            cid: 0,
            guid: '',
            uid: $.cookie('uid')
        }
        var dialog = bootbox.dialog({
            message: '<p><i class="fa fa-spin fa-spinner"></i> por favor espere...</p>',
            size: 'small',
            closeButton: false
        });

        $.ajax({
            type: 'POST',
            data: JSON.stringify(params),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: true,
            url: 'services/expedientedigital.asmx/searchrefvisor',
            success: function (data) {
                var datajs = $.parseJSON(data.d);
                $('#contentpanel').html("");
                console.log(datajs);
                if (datajs.length > 0) {

                    $('#txtref').val(ref);
                    $('#txtnumped').val(datajs[0].numped);
                    $('#clienteid').val(datajs[0].clienteid);
                    $('#contentpanel').html(datajs[0].htmlbody);
                    $('#ctrlfacturas').val(datajs[0].ctrlfacturas);
                    $('#ctrlconsolidados').val(datajs[0].ctrlconsolidados);
                    $('#ctrlpreentrada').val(datajs[0].ctrlpreentrada);
                    $("#successmsg").text("Referencia " + ref + " correcta\nCargando expediente digital");
                    $("#messageNotification").jqxNotification("open");
                    p._openfilesbyref();
                }
                else {
                    var initpanel = "<div style='margin: 10px;'><span class='text-bold'>  Referencia : </span><br />";
                    initpanel += "<span class='text-bold'>Cliente : </span><br />";
                    initpanel += "<span class='text-bold'>Proveedor : </span><br />";
                    initpanel += "<span class='text-bold'>Pedimento : </span><br />";
                    initpanel += "<span class='text-bold'>Facturas : </span><br /></div>";
                    initpanel += "<span class='text-bold'>Consolidados : </span><br /></div>";
                    $('#contentpanel').html(initpanel);
                    $("#warningmsg").text("La referencia " + ref + " no existe");
                    $("#warningnotification").jqxNotification("open");
                }
            }, error: function (xhr, textStatus, error) {
                var errorM = $.parseJSON(xhr.responseText);
                $("#errormsg").text(errorM.Message);
                $("#errorTimeOutNotification").jqxNotification("open");
            }, complete: function () {
                dialog.modal('hide');
            }
        });
    };

    p._upploadfilesbyref = function () {
        $('#formfileupload').modal('hide');

        var randomnumber = Math.floor(Math.random() * 10001);
        var form_data = new FormData();
        var ref = $("#inputref").val().toUpperCase();
        if (ref == "") {
            $("#warningmsg").text("No ha tecleado referencia !!");
            $("#warningnotification").jqxNotification("open");
            this.removeFile(file);
            return;
        }
        form_data.append('ref', ref);
        var fileexpire = $('#dateexp').prop('checked');
        if (fileexpire) {
            var selection = $("#jqxwidgetdatetime").jqxDateTimeInput('getRange');
            if (selection.from == null) {
                $("#warningmsg").text("Atencion !! ha seleccionado aplicar vigencia, pero aun no selecciona rango de fechas");
                $("#warningnotification").jqxNotification("open");
                this.removeFile(file);
            }
            form_data.append('fileexpire', fileexpire);
            form_data.append('datefrom', selection.from.toLocaleDateString());
            form_data.append('dateto', selection.to.toLocaleDateString());
        }
        var clientaccess = $('#clientaccess').prop('checked');
        form_data.append('clientaccess', clientaccess);
        var comments = $("#jqxTextArea").val();
        form_data.append('comments', comments);
        var doctype = $("#selectTipo").val();
        form_data.append('doctype', doctype);
        form_data.append('user', $.cookie('uname'));

        var dialog = bootbox.dialog({
            message: '<p><i class="fa fa-spin fa-spinner"></i> por favor espere...</p>',
            size: 'small',
            closeButton: false
        });

        //we set the status saying the site is currently uploading the file. Note: you can remove the image ajax animated loading if you do not have it and just leave the text information base only. Make sure you set the URL for the generic handler path correctly.
        $.ajax({
            url: "services/expedientedigital.asmx/UploadExpFile",
            cache: false,
            contentType: false,
            processData: false,
            data: form_data,
            type: 'post',
            success: function (response) {
                p._openfilesbyref();
                $("#successmsg").text("El archivo se importo exitosamente");
                $("#messageNotification").jqxNotification("open");
                $("#dateexp").checked = false;
                $("#clientaccess").checked = true;
                $("#jqxTextArea").val("");
                Dropzone.forElement("#mydropzone").removeAllFiles(true);
            }, error: function (xhr, textStatus, error) {
                var errorM = $.parseJSON(xhr.responseText);
                $("#errormsg").text(errorM.Message);
                $("#errorTimeOutNotification").jqxNotification("open");
            }, complete: function () {
                dialog.modal('hide');
            }
        });
    };
    var jsongrid;
    p._loadGridTable = function () {
        var dialog = bootbox.dialog({
            message: '<p><i class="fa fa-spin fa-spinner"></i> por favor espere...</p>',
            size: 'small',
            closeButton: false
        });
        var source =
        {
            localData: jsongrid,
            dataType: "json",
            dataFields:
            [
                { name: 'idfilename', type: 'string' },
                { name: 'content_type', type: 'string' },
                { name: 'key', type: 'string' },
                { name: 'doctypename', type: 'string' },
                { name: 'docdate', type: 'string' },
                { name: 'content_type', type: 'string' },
                { name: 'user', type: 'string' },
                { name: 'comments', type: 'string' }
            ]
        };
        var dataAdapter = new $.jqx.dataAdapter(source);

        $("#jqxgrid").jqxGrid(
        {
            width: '100%',
            height: 800,
            filterable: true,
            altRows: true,
            theme: theme,
            showfilterrow: true,
            sortable: true,
            showtoolbar: true,
            showstatusbar: true,
            source: dataAdapter,
            rendertoolbar: function (toolbar) {
                var me = this;
                var html = '';
                html += '<div style="margin: 5px;">';
                html += '	<input type="button" value="Exportar a Excel" id="excelExport" />';
                html += '</div>';
                var $new = $(html);
                toolbar.append($new);
                $("#excelExport").jqxButton({ theme: 'bootstrap' });
                $("#excelExport").on('click', function () {
                    $("#jqxgrid").jqxGrid('exportdata', 'xls', 'jqxGrid', true, null, true, 'services/ExportContent.asmx/ExcelExport');
                });

                var contextMenu = $("#Menu").jqxMenu({ width: 200, height: 30, autoOpenPopup: false, mode: 'popup', theme: theme });
                $("#jqxgrid").on('contextmenu', function () {
                    return false;
                });
                $("#Menu").on('itemclick', function (event) {
                    var args = event.args;
                    var rowindex = $("#jqxgrid").jqxGrid('getselectedrowindex');
                    if ($.trim($(args).text()) == "Abrir archivo") {
                        var dataRecord = $("#jqxgrid").jqxGrid('getrowdata', rowindex);
                        var filename = dataRecord.idfilename;
                        var fileext = dataRecord.content_type;
                        p._opensinglefile(filename, fileext);
                    }
                });
                $("#jqxgrid").on('rowclick', function (event) {
                    if (event.args.rightclick) {
                        $("#jqxgrid").jqxGrid('selectrow', event.args.rowindex);
                        var scrollTop = $(window).scrollTop();
                        var scrollLeft = $(window).scrollLeft();
                        contextMenu.jqxMenu('open', parseInt(event.args.originalEvent.clientX) + 5 + scrollLeft, parseInt(event.args.originalEvent.clientY) + 5 + scrollTop);
                        return false;
                    }
                });
            },
            columns: [
              { dataField: 'idfilename', hidden: true },
              { text: 'Referencia', dataField: 'key', width: 100 },
              { text: 'Documento', dataField: 'doctypename', width: 250 },
              { text: 'Fecha Actualizacion', dataField: 'docdate', width: 200 },
              { text: 'Formato de archivo', dataField: 'content_type', width: 100 },
              { text: 'Usuario', dataField: 'user', width: 150 },
              { text: 'Comentarios', dataField: 'comments' }
            ]
        });

        //$("#jqxgrid").jqxGrid({ source: dataAdapter });
        dialog.modal('hide');

    };
    p._openfilesbyref = function () {
        $('#formOpenFile').modal('hide');
        var ref = $("#inputref").val().toUpperCase();
        if (ref.length == 0) {
            $("#warningmsg").text("No ha tecleado referencia !!");
            $("#warningnotification").jqxNotification("open");
            return;
        }
        var facturas = $('#ctrlfacturas').val();
        var consolidados = $('#ctrlconsolidados').val();
        var preentrada = $('#ctrlpreentrada').val();
        var postdata = { referencia: ref, facturas: facturas, consolidados: consolidados, preentrada: preentrada };
        var dialog = bootbox.dialog({
            message: '<p><i class="fa fa-spin fa-spinner"></i> por favor espere...</p>',
            size: 'small',
            closeButton: false
        });
        $.ajax({
            type: 'POST',
            data: JSON.stringify(postdata),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: true,
            url: 'services/expedientedigital.asmx/getexpedientearray',
            complete: function () {
                dialog.modal('hide');
            }, error: function (xhr, textStatus, error) {

                var errorM = $.parseJSON(xhr.responseText);
                $("#errormsg").text(errorM.Message);
                $("#errorTimeOutNotification").jqxNotification("open");
            },
            success: function (data) {
                $("#loading").hide();
                var jsondata = $.parseJSON(data.d);

                $("#ctrlref").val(ref);

                jsongrid = jsondata;


                $("#dochistorial").empty();
                var container = "";
                var idoc = 0;
                var currenttype = "";
                var oldtype = "";
                container += '<div class="row">';

                $.each(jsondata, function (key, value) {

                    if (currenttype != value.doctype) {
                        currenttype = value.doctype;
                        if (container != "")
                            container += "</div><hr /><div class='row'>";

                    }

                    var imgurl = "";
                    var ext = value.content_type.toLowerCase();
                    if (ext == "xls" || ext == "xlsx")
                        imgurl = "html/excel.png";
                    else if (ext == "doc" || ext == "docx")
                        imgurl = "html/word.png";
                    else if (ext == "ppt" || ext == "pptx")
                        imgurl = "html/word.png";
                    else if (ext == "xml")
                        imgurl = "html/xml.png";
                    else if (ext == "pdf" || ext == "jpg" || ext == "png" || ext == ".png" || ext == "bmp" || ext == "gif" || ext == "tiff")
                        imgurl = "thumbs/thumb_" + value.idfilename + ".jpeg";
                    else
                        imgurl = "html/document.png";

                    container += '<div class = "col-md-3">';
                    container += "<div class='checkbox checkbox-styled' name='foo'><label><input type='checkbox' value=''  data-filename='" + value.idfilename + "' data-fullfilename='" + value.doctypename + "_" + value.idfilename + "'><span>Seleccionar</span></label></div>";
                    container += '      <a href="#" data-thumb="" data-filename=' + value.idfilename + ' data-fileext=' + ext + '>';
                    container += '  <div>';
                    container += '          <img style="width:100px; height:120px;" src = "' + imgurl + '" alt = "' + value.idfilename + '">';
                    container += '   </div>';
                    container += '   <div class = "caption">';
                    container += '      <h5>' + value.doctypename + '</h5>';
                    container += "      <div><b>Fecha de actualizacion: </b>" + value.docdate + "</div>";
                    container += "      <div><b>Expira: </b>" + value.expdate + "</div>";
                    container += "      <div><b>Commentarios: </b>" + value.comments + "</div></div></a>";
                    container += '   <div class="btn-group btn-group" role="group" aria-label="Justified button group">';
                    container += '     <button class="buydesc btn btn-sm btn-default" role="button" data-filename="' + value.idfilename + '" data-filefullname="' + value.doctypename + "_" + value.idfilename + '" title="Descargar Archivo">';
                    container += '        Descargar';
                    container += '     </button> ';
                    container += '     <button class="buysend btn btn-sm btn-default" role="button" data-filename="' + value.idfilename + '" data-filefullname="' + value.doctypename + "_" + value.idfilename + '" title="Email Archivo">';
                    container += '        Email';
                    container += '     </button>';
                    container += '     <button class="buyelimina btn btn-sm btn-default" role="button" data-filename="' + value.idfilename + '" data-filefullname="' + value.doctypename + "_" + value.idfilename + '" title="Eliminar Archivo">';
                    container += '        Eliminar';
                    container += '     </button>';
                    container += '   </div>';
                    container += '</div>';

                    idoc++;
                });

                container += '</div><hr />';


                $("#dochistorial").append(container);
                $("#tabexpo").tab('show');
                $("#tabimpo").tab('show');
                //$("#jqxgrid").jqxGrid('applyfilters');
                //$('#jqxgrid').jqxGrid('removefilter');
                //$('#jqxgrid').jqxGrid({ filtermode: 'default' });
                //$('#jqxgrid').jqxGrid('clearfilters');

                $('[data-thumb]').on("click", function () {
                    var filename = $(this).data("filename");
                    var fileext = $(this).data("fileext");
                    p._opensinglefile(filename, fileext);
                    return false;
                });

                $(".buydesc").click(function (e) {
                    var txt = $(e.target).data('filename');
                    var txtfull = $(e.target).data('filefullname');
                    p._downloadsinglefile(txt, txtfull);
                });

                $(".buysend").click(function (e) {
                    var email = $.cookie('uemail');
                    $('#emailto').val(email);
                    var $this = $(this);
                    var offset = $this.offset();
                    $('#emailWindow').jqxWindow({ position: { x: offset.left + 10, y: offset.top + 10 }, });
                    $('#emailWindow').jqxWindow('open');
                    var txt = $(e.target).data('filename');
                    var txtfull = $(e.target).data('filefullname');
                    $("#cmdsendsingle").val(txt);
                    $("#cmdsendsinglefull").val(txtfull);
                });

                $(".buyelimina").click(function (e) {
                    var $this = $(this);
                    var offset = $this.offset();
                    $('#deletewindow').jqxWindow({ position: { x: offset.left + 10, y: offset.top + 10 }, });
                    $('#deletewindow').jqxWindow('open');
                    var txt = $(e.target).data('filename');
                    var txtfull = $(e.target).data('filefullname');
                    $("#cmddeletesingle").val(txt);
                    $("#cmddeletesinglefull").val(txtfull);
                });

                $("#successmsg").text("Expediente cargado exitosamente");
                $("#messageNotification").jqxNotification("open");
            }
        });
    };

    function groupBy(array, f) {
        var groups = {};
        array.forEach(function (o) {
            var group = JSON.stringify(f(o));
            groups[group] = groups[group] || [];
            groups[group].push(o);
        });
        return Object.keys(groups).map(function (group) {
            return groups[group];
        })
    }
    var DataGrouper = (function () {
        var has = function (obj, target) {
            return _.any(obj, function (value) {
                return _.isEqual(value, target);
            });
        };

        var keys = function (data, names) {
            return _.reduce(data, function (memo, item) {
                var key = _.pick(item, names);
                if (!has(memo, key)) {
                    memo.push(key);
                }
                return memo;
            }, []);
        };

        var group = function (data, names) {
            var stems = keys(data, names);
            return _.map(stems, function (stem) {
                return {
                    key: stem,
                    vals: _.map(_.where(data, stem), function (item) {
                        return _.omit(item, names);
                    })
                };
            });
        };

        group.register = function (name, converter) {
            return group[name] = function (data, names) {
                return _.map(group(data, names), converter);
            };
        };

        return group;
    }());

    p._opensinglefile = function (filename, ext) {
        if (filename == 0) {
            $("#warningmsg").text("No ha seleccionado archivo");
            $("#warningnotification").jqxNotification("open");
            return;
        }
        var dialog = bootbox.dialog({
            message: '<p><i class="fa fa-spin fa-spinner"></i> por favor espere...</p>',
            size: 'small',
            closeButton: false
        });
        var ref = $("#ctrlref").val().toUpperCase();
        var postdata = { filename: filename, _ref: ref }
        $.ajax({
            type: 'POST',
            data: JSON.stringify(postdata),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: true,
            url: 'services/expedientedigital.asmx/opensinglefile',
            success: function (data) {
                var jsondata = $.parseJSON(data.d);
                var URL = "tempdigitalfiles/" + jsondata.fullfilename;
                window.open(URL);
            }, error: function (xhr, textStatus, error) {
                var errorM = $.parseJSON(xhr.responseText);
                $("#errormsg").text(errorM.Message);
                $("#errorTimeOutNotification").jqxNotification("open");
            }, complete: function () {
                dialog.modal('hide');
            }
        });
    };
    p._downloadsinglefile = function (filename, fullfilename) {
        if (filename == 0) {
            $("#warningmsg").text("No ha seleccionado archivo");
            $("#warningnotification").jqxNotification("open");
            return;
        }
        var dialog = bootbox.dialog({
            message: '<p><i class="fa fa-spin fa-spinner"></i> por favor espere...</p>',
            size: 'small',
            closeButton: false
        });
        var ref = $("#ctrlref").val().toUpperCase();
        var postdata = { filename: filename, fullfilename: fullfilename, _ref: ref }
        $.ajax({
            type: 'POST',
            data: JSON.stringify(postdata),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: true,
            url: 'services/expedientedigital.asmx/downloadsinglefile',
            success: function (data) {
                var url = "tempdigitalfiles/" + data.d;
                $('#framevisor').attr('src', url);
            }, error: function (xhr, textStatus, error) {
                var errorM = $.parseJSON(xhr.responseText);
                $("#errormsg").text(errorM.Message);
                $("#errorTimeOutNotification").jqxNotification("open");
            }, complete: function () {
                dialog.modal('hide');
            }
        });
    };
    p._downloadSelected = function () {

        var filestodwn = "";
        var fullfilestodwn = "";
        $("input[type='checkbox']").each(function () {
            if (this.checked == true) {
                if ($(this).data('filename') != undefined) {
                    if (filestodwn == "") {
                        filestodwn += $(this).data('filename');
                        fullfilestodwn += $(this).data('fullfilename');
                    }
                    else {
                        filestodwn += "|" + $(this).data('filename');
                        fullfilestodwn += "|" + $(this).data('fullfilename');
                    }
                }
            }
        });

        if (filestodwn == "") {
            $("#warningmsg").text("No ha seleccionado archivo");
            $("#warningnotification").jqxNotification("open");
            return;
        }
        var dialog = bootbox.dialog({
            message: '<p><i class="fa fa-spin fa-spinner"></i> por favor espere...</p>',
            size: 'small',
            closeButton: false
        });
        var ref = $("#ctrlref").val().toUpperCase();
        var postdata = { files: filestodwn, fullfilenames: fullfilestodwn, _ref: ref }
        $.ajax({
            type: 'POST',
            data: JSON.stringify(postdata),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: true,
            url: 'services/expedientedigital.asmx/downloadSelected',
            success: function (data) {
                var url = "tempdigitalfiles/" + data.d;
                $('#framevisor').attr('src', url);
            }, error: function (xhr, textStatus, error) {
                var errorM = $.parseJSON(xhr.responseText);
                $("#errormsg").text(errorM.Message);
                $("#errorTimeOutNotification").jqxNotification("open");
            }, complete: function () {
                dialog.modal('hide');
            }
        });

    };
    p._sendSelected = function () {
        $('#emailWindow').jqxWindow('close');
        var filestodwn = "";
        var fullfilestodwn = "";
        $("input[type='checkbox']").each(function () {
            if (this.checked == true) {
                if ($(this).data('filename') != undefined) {
                    if (filestodwn == "") {
                        filestodwn += $(this).data('filename');
                        fullfilestodwn += $(this).data('fullfilename');
                    }
                    else {
                        filestodwn += "|" + $(this).data('filename');
                        fullfilestodwn += "|" + $(this).data('fullfilename');
                    }
                }
            }
        });
        if (filestodwn == "") {
            filestodwn += $("#cmdsendsingle").val();
            fullfilestodwn += $("#cmdsendsinglefull").val();
        }
        else {
            filestodwn += "|" + $("#cmdsendsingle").val();
            fullfilestodwn += "|" + $("#cmdsendsinglefull").val();
        }

        if (filestodwn == "") {
            $("#warningmsg").text("No ha seleccionado archivo");
            $("#warningnotification").jqxNotification("open");
            return;
        }
        var mailto = $("#emailto").val();
        if (mailto == "") {
            $("#warningmsg").text("No ha tecleado email!!");
            $("#warningnotification").jqxNotification("open");
            return;
        }
        var dialog = bootbox.dialog({
            message: '<p><i class="fa fa-spin fa-spinner"></i> por favor espere...</p>',
            size: 'small',
            closeButton: false
        });
        var ref = $("#ctrlref").val().toUpperCase();;
        var postdata = { files: filestodwn, mailto: mailto, fullfilenames: fullfilestodwn, _ref: ref }
        $.ajax({
            type: 'POST',
            data: JSON.stringify(postdata),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: true,
            url: 'services/expedientedigital.asmx/sendSelected',
            success: function (data) {
                $("#successmsg").text("Archivos enviados exitosamente!!");
                $("#messageNotification").jqxNotification("open");
            }, error: function (xhr, textStatus, error) {
                var errorM = $.parseJSON(xhr.responseText);
                $("#errormsg").text(errorM.Message);
                $("#errorTimeOutNotification").jqxNotification("open");
            }, complete: function () {
                dialog.modal('hide');
            }
        });
    };
    p._deleteSinglefile = function () {
        var filename = $("#cmddeletesingle").val();
        var pass = $("#jqxPasswordInput").val();
        if (filename == "") {
            $("#warningmsg").text("No ha seleccionado archivo");
            $("#warningnotification").jqxNotification("open");
            return;
        }
        if (pass == "") {
            $("#warningmsg").text("No ha ingresado password");
            $("#warningnotification").jqxNotification("open");
            return;
        }
        var dialog = bootbox.dialog({
            message: '<p><i class="fa fa-spin fa-spinner"></i> por favor espere...</p>',
            size: 'small',
            closeButton: false
        });
        var ref = $("#ctrlref").val().toUpperCase();
        var postdata = { filename: filename, _ref: ref, pass: pass }
        $.ajax({
            type: 'POST',
            data: JSON.stringify(postdata),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: true,
            url: 'services/expedientedigital.asmx/deleteSingleFile',
            success: function (data) {
                var jsdata = data.d;

                $("#cmddeletesingle").val('');
                $("#jqxPasswordInput").val('');
                $("#successmsg").text("Archivo eliminado exitosamente!!");
                $("#messageNotification").jqxNotification("open");

                p._openfilesbyref();
            }, error: function (xhr, textStatus, error) {
                var errorM = $.parseJSON(xhr.responseText);
                $("#errormsg").text(errorM.Message);
                $("#errorTimeOutNotification").jqxNotification("open");
            }, complete: function () {
                dialog.modal('hide');
            }
        });
    };
    p._copyprofile = function (target) {
        var ref = $("#ctrlref").val().toUpperCase();
        if (ref == "") {
            $("#warningmsg").text("No ha abierto ninguna referencia");
            $("#warningnotification").jqxNotification("open");
            return;
        }
        if (target == "") {
            $("#warningmsg").text("Referencia invalida");
            $("#warningnotification").jqxNotification("open");
            return;
        }
        var dialog = bootbox.dialog({
            message: '<p><i class="fa fa-spin fa-spinner"></i> por favor espere...</p>',
            size: 'small',
            closeButton: false
        });
        var postdata = { sourceref: ref, targetref: target }
        $.ajax({
            type: 'POST',
            data: JSON.stringify(postdata),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: true,
            url: 'services/expedientedigital.asmx/copyprofile',
            success: function (data) {

                $("#successmsg").text("Los archivos se han copiado exitosamente!!");
                $("#messageNotification").jqxNotification("open");

            }, error: function (xhr, textStatus, error) {
                var errorM = $.parseJSON(xhr.responseText);
                $("#errormsg").text(errorM.Message);
                $("#errorTimeOutNotification").jqxNotification("open");
            }, complete: function () {
                dialog.modal('hide');
            }
        });
    };
    namespace.expedientedigital = new expedientedigital;
}(this.materialadmin, jQuery)); // pass in (namespace, jQuery):






