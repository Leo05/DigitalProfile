<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="expedientedigital.aspx.cs" Inherits="SMIWeb.expedientedigital" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <title><%=ConfigurationManager.AppSettings["CIA.NAME"]%> - Expediente Digital</title>
    <link rel="shortcut icon" href="assets/img/favicon.ico">

    <!-- BEGIN META -->
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="keywords" content="your,keywords">
    <meta name="description" content="Short explanation about this website">
    <!-- END META -->

    <!-- BEGIN STYLESHEETS -->
    <link href='http://fonts.googleapis.com/css?family=Roboto:300italic,400italic,300,400,500,700,900' rel='stylesheet' type='text/css' />
    <link type="text/css" rel="stylesheet" href="assets/css/theme-3/bootstrap.css?1422792965" />
    <link type="text/css" rel="stylesheet" href="assets/css/theme-3/materialadmin.css?1425466319" />
    <link type="text/css" rel="stylesheet" href="assets/css/theme-3/font-awesome.min.css?1422529194" />
    <link type="text/css" rel="stylesheet" href="assets/css/theme-3/material-design-iconic-font.min.css?1421434286" />
    <link type="text/css" rel="stylesheet" href="assets/css/theme-3/libs/dropzone/dropzone-theme.css" />
    <link rel="stylesheet" href="assets/js/libs/jqwidgets/styles/jqx.base.css" type="text/css" />
    <link href="assets/js/libs/jqwidgets/styles/jqx.arctic.css" rel="stylesheet" />
    <style>
        .jqx-notification-container {
            z-index: 9999;
        }
    </style>

    <!-- END STYLESHEETS -->

    <!-- HTML5 shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!--[if lt IE 9]>
		<script type="text/javascript" src="assets/js/libs/utils/html5shiv.js?1403934957"></script>
		<script type="text/javascript" src="assets/js/libs/utils/respond.min.js?1403934956"></script>
		<![endif]-->
</head>
<body class="menubar-hoverable header-fixed ">
    <audio src="sounds/tada.wav" id="sound1"></audio>
    <div id="jqxLoader">
    </div>

    <input type="hidden" id="clienteid" />
    <input type="hidden" id="txtref" />
    <input type="hidden" id="txtnumped" />
    <input type="hidden" id="txtfacid" />

    <!-- BEGIN HEADER-->
    <header id="header">
        <div class="headerbar">
            <!-- Brand and toggle get grouped for better mobile display -->
            <div class="headerbar-left">
                <ul class="header-nav header-nav-options">
                    <li class="header-nav-brand">
                        <div class="brand-holder">
                            <a href="#">
                                <span class="text-lg text-bold text-primary"><%=ConfigurationManager.AppSettings["CIA.NAME"]%></span>
                            </a>
                        </div>
                    </li>
                    <li>
                        <a class="btn btn-icon-toggle menubar-toggle" data-toggle="menubar" href="javascript:void(0);">
                            <i class="fa fa-bars"></i>
                        </a>
                    </li>
                </ul>
            </div>
            <!-- Collect the nav links, forms, and other content for toggling -->
            <div class="headerbar-right">
                <ul class="header-nav header-nav-options">
                    <li class="dropdown hidden-xs">
                        <a href="javascript:void(0);" class="btn btn-icon-toggle btn-default" data-toggle="dropdown" id="msgalert">
                            <i class="fa fa-bell"></i><sup id="submsgalert" class=""></sup>
                        </a>
                        <ul id="menumsg" class="dropdown-menu animation-expand">
                        </ul>
                        <!--end .dropdown-menu -->
                    </li>
                    <!--end .dropdown -->
                </ul>
                <!--end .header-nav-options -->
                <ul class="header-nav header-nav-profile">
                    <li class="dropdown">
                        <a href="javascript:void(0);" class="dropdown-toggle ink-reaction" data-toggle="dropdown">
                            <img src="assets/img/avatar1.jpg?1403934956" alt="" />
                            <span id="profile-info" class="profile-info">Usuario
									<small>Administrator</small>
                            </span>
                        </a>
                        <ul class="dropdown-menu animation-dock">
                            <li class="dropdown-header">Config</li>
                            <li><a href="#">My profile</a></li>
                            <li class="divider"></li>
                            <li><a href="locked.aspx"><i class="fa fa-fw fa-lock"></i>Lock</a></li>
                            <li><a href="login.aspx"><i class="fa fa-fw fa-power-off text-danger"></i>Logout</a></li>
                        </ul>
                        <!--end .dropdown-menu -->
                    </li>
                    <!--end .dropdown -->
                </ul>
                <!--end .header-nav-profile -->
                <ul class="header-nav header-nav-toggle">
                    <li>
                        <a id="btnSearch" class="btn btn-icon-toggle btn-default" href="#offcanvas-search" data-toggle="offcanvas" data-backdrop="false">
                            <i class="md md-textsms"></i>
                        </a>
                    </li>
                </ul>
                <!--end .header-nav-toggle -->
            </div>
            <!--end #header-navbar-collapse -->
        </div>
    </header>
    <!-- END HEADER-->

    <!-- BEGIN BASE-->
    <div id="base">

        <!-- BEGIN OFFCANVAS LEFT -->
        <div class="offcanvas">
        </div>
        <!--end .offcanvas-->
        <!-- END OFFCANVAS LEFT -->

        <!-- BEGIN CONTENT-->
        <div id="content">
            <section class="style-default-bright">
                <br />
                <div id="messageNotification">
                    <div><span id="successmsg"></span></div>
                </div>
                <div id="errorTimeOutNotification">
                    <div><span id="errormsg"></span></div>
                </div>
                <div id="warningnotification">
                    <div><span id="warningmsg"></span></div>
                </div>
                <div class="row">
                    <div class="col-sm-12">
                        <div id="jqxToolBar">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-12">
                        <div id="contentexpander">
                            <div>Detalles</div>
                            <div id="contentpanel">
                                <div style="margin-left: 10px">
                                    <br />
                                    <span class='text-bold'>Referencia : </span>
                                    <br />
                                    <span class='text-bold'>Cliente : </span>
                                    <br />
                                    <span class='text-bold'>Proveedor : </span>
                                    <br />
                                    <span class='text-bold'>Pedimento : </span>
                                    <br />
                                    <span class='text-bold'>Facturas : </span>
                                    <br />
                                    <br />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12">
                        <div class="card card-underline">
                            <div class="card-head">
                                <ul class="nav nav-tabs pull-left" data-toggle="tabs">
                                    <li class="active"><a id="tabimpo" href="#first2">VISTAS MINIATURA</a></li>
                                    <li><a id="tabexpo" href="#second2">VISTA DETALLE</a></li>
                                </ul>
                            </div>
                            <div class="card-body scroll style-default-bright tab-content" style="height: 800px">
                                <div class="tab-pane default" id="first2">
                                    <div id="dochistorial"></div>
                                </div>
                                <div class="tab-pane" id="second2">
                                    <div id='jqxgrid'>
                                    </div>
                                    <div class="alert alert-info" role="alert">
                                        <i class="fa fa-lightbulb-o"></i><strong>Informacion:</strong> Click derecho en algún registro para abrir
                                    </div>
                                </div>
                            </div>
                        </div>
                        <!--end .card -->
                    </div>
                    <!--end .col -->
                </div>
            </section>
        </div>

        <!--end #content-->
        <!-- END CONTENT -->

        <!-- BEGIN MENUBAR-->
        <div id="menubar" class="menubar-inverse ">
            <div class="menubar-fixed-panel">
                <div>
                    <a class="btn btn-icon-toggle btn-default menubar-toggle" data-toggle="menubar" href="javascript:void(0);">
                        <i class="fa fa-bars"></i>
                    </a>
                </div>
                <div class="expanded">
                    <a href="dashboardjqx.aspx">
                        <span class="text-lg text-bold text-primary "><%=ConfigurationManager.AppSettings["CIA.NAME"]%></span>
                    </a>
                </div>
            </div>
            <div class="menubar-scroll-panel">
                <!-- BEGIN MAIN MENU -->
                <ul id="main-menu" class="gui-controls">
                </ul>
                <div id="txtcia" class="menubar-foot-panel">
                </div>
            </div>
            <!--end .menubar-scroll-panel-->
        </div>
        <!--end #menubar-->
        <!-- END MENUBAR -->

        <!-- BEGIN OFFCANVAS RIGHT -->
        <div class="offcanvas">
            <!-- BEGIN OFFCANVAS SEARCH -->
            <div id="offcanvas-search" class="offcanvas-pane width-8">
                <div class="offcanvas-head">
                    <header class="text-primary">Search</header>
                    <div class="offcanvas-tools">
                        <a class="btn btn-icon-toggle btn-default-light pull-right" data-dismiss="offcanvas">
                            <i class="md md-close"></i>
                        </a>
                    </div>
                </div>
                <form class="navbar-search" role="search">
                    <div class="form-group">
                        <input id="cmdfinduser" type="text" class="form-control" name="headerSearch" placeholder="Enter your keyword">
                    </div>
                    <button type="submit" class="btn btn-icon-toggle ink-reaction"><i class="fa fa-search"></i></button>
                </form>
                <div class="offcanvas-body no-padding">
                    <ul id="usertable" class="list">
                    </ul>
                </div>
                <!--end .offcanvas-body -->
            </div>
            <!--end .offcanvas-pane -->
            <!-- END OFFCANVAS SEARCH -->

            <!-- BEGIN OFFCANVAS CHAT -->
            <div id="offcanvas-chat" class="offcanvas-pane style-default-light width-12">
                <div class="offcanvas-head style-default-bright">
                    <header class="text-primary"><span id="chatHeader">CHAT</span></header>
                    <div class="offcanvas-tools">
                        <a id="btnToggle" class="btn btn-icon-toggle btn-default-light pull-right" data-dismiss="offcanvas">
                            <i class="md md-close"></i>
                        </a>
                        <a id="btnToggleContinue" class="btn btn-icon-toggle btn-default-light pull-right" href="#offcanvas-search" data-toggle="offcanvas" data-backdrop="false">
                            <i class="md md-arrow-back"></i>
                        </a>
                        <a id="refreshchat" class="btn btn-icon-toggle btn-refresh btn-default-light pull-right"><i class="md md-refresh"></i></a>
                    </div>
                    <form class="form">
                        <div class="form-group floating-label">
                            <textarea name="sidebarChatMessage" id="sidebarChatMessage" class="form-control autosize" rows="1"></textarea>
                            <label for="sidebarChatMessage">Deja tu mensaje</label>
                        </div>
                    </form>
                </div>
                <div class="offcanvas-body">
                    <ul class="list-chats">
                    </ul>
                </div>
                <!--end .offcanvas-body -->
            </div>
            <!--end .offcanvas-pane -->
            <!-- END OFFCANVAS CHAT -->
        </div>
        <!--end .offcanvas-->
        <!-- END OFFCANVAS RIGHT -->

        <div class="modal fade" id="formfileupload" tabindex="-1" role="dialog" aria-labelledby="formModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        <h4 class="modal-title" id="H2">Seleccione tipo de documentos a importar</h4>
                    </div>
                    <div class="form-horizontal" role="form">
                        <div class="modal-body">
                            <div class="form-group">
                                <div class="col-sm-12">
                                    <select id="selectTipo" name="selectTipo" class="form-control">
                                    </select>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-sm-5">
                                    <div class="input-daterange input-group" id="demo-date-range">
                                        <div class="input-group-content">
                                            <div id='jqxwidgetdatetime'>
                                            </div>
                                            <div class='checkbox-styled'>
                                                <label>
                                                    <input type='checkbox' value='' id="dateexp" /><span>Aplicar Vigencia</span>
                                                </label>
                                            </div>
                                            <div class='checkbox-styled'>
                                                <label>
                                                    <input type='checkbox' value='' id="clientaccess" checked /><span>Acceso a clientes</span>
                                                </label>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-7">
                                    <textarea id="jqxTextArea"></textarea>
                                </div>
                            </div>
                            <hr class="ruler-xl" />
                            <div class="form-group">
                                <div class="input-group">
                                    <div class="input-group-content">
                                        <form id="mydropzone" class="dropzone" action="#" method="post" runat="server" enctype="multipart/form-data">
                                            <div class="dz-default dz-message"><span>Arrastre los archivos a subir o de click en esta seccion</span></div>
                                            <div class="fallback" hidden>
                                                <input id="filessent" name="fil" type="file" multiple />
                                            </div>
                                            <div id="dz-preview">
                                                <div>
                                                    <div><span id="data-dz-name"></span></div>
                                                    <div><span id="data-dz-size"></span></div>
                                                </div>
                                                <div><span id="data-dz-uploadprogress"></span></div>
                                                <div><span id="data-dz-errormessage"></span></div>
                                            </div>
                                        </form>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button id="btnenviarcomments" type="button" class="btn btn-primary">Subir</button>
                            <button type="button" class="btn btn-default" data-dismiss="modal">Cancelar</button>
                        </div>
                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>
        <!-- BEGIN FORM MODAL MARKUP -->
        <div class="modal fade" id="formOpenFile" tabindex="-1" role="dialog" aria-labelledby="formModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        <h4 class="modal-title" id="formModalLabel">Abrir expediente digital</h4>
                    </div>
                    <div class="form-horizontal" role="form">
                        <div class="modal-body">
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                        </div>
                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>
        <div class="modal fade" id="formctagastos" tabindex="-1" role="dialog" aria-labelledby="formModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        <h4 class="modal-title" id="H4">Notificacion de cuenta de gastos a clientes</h4>
                    </div>
                    <div class="modal-body">
                        <div class="row">
                            <div class="col-md-12">
                                <div class="form-group">
                                    <label for="txtctefac">Cliente de facturacion:</label>
                                    <h4>
                                        <strong>
                                            <label id="txtctefac"></label>
                                        </strong>
                                    </h4>
                                </div>
                                <div class="form-group">
                                    <label for="table">Confirmar correos electonicos de clientes</label>
                                    <div id="table"></div>
                                </div>
                                <div class="form-group">
                                    <label for="txtsubject">Titulo del correo</label>
                                    <input type="text" class="form-control" id="txtsubject">
                                </div>
                                <div class="form-group">
                                    <div class="checkbox checkbox-styled">
                                        <label>
                                            <input type="checkbox" value="" id="cbcxcmergefiles" checked>
                                            <span>Juntar archivos PDF en un solo archivo</span>
                                        </label>
                                    </div>
                                    <div class="checkbox checkbox-styled">
                                        <label>
                                            <input type="checkbox" value="" id="cbcxccompressfiles" checked>
                                            <span>Comprimir archivos en formato ZIP</span>
                                        </label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal">Cancelar</button>
                        <button id="btnenviarctagastos" type="button" class="btn btn-primary">Enviar</button>
                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>
        <!-- /.modal -->
        <div id="window" hidden>
            <div id="windowHeader">
                <span>
                    <img src="#" alt="" style="margin-right: 15px" />
                </span>
            </div>
            <div style="overflow: hidden;" id="windowContent">
                <div id="mainSplitter">
                    <div style="width: 300px" id="filedetails" class="splitter-panel">
                    </div>
                    <div class="splitter-panel">
                        <iframe id="framevisor" src="#" style="width: 100%; height: 100%;"></iframe>
                    </div>
                </div>
            </div>
        </div>
        <div id="emailWindow">
            <div id="customWindowHeader">
                <span id="captureContainer" style="float: left">Enviar archivos por email</span>
            </div>
            <div id="customWindowContent" style="overflow: hidden">
                <div style="margin: 10px">
                    Email:
                        <input type="text" id="emailto" />
                    <div style="float: right">
                        <input type="button" value="Enviar" style="margin-bottom: 5px;" id="searchTextButton" /><br />
                        <input type="button" value="Cancelar" id="cancelButton" />
                    </div>
                    <br />
                    <br />
                    <div id="matchCaseCheckBox">
                        Incluir contactos del cliente
                    </div>
                </div>
            </div>
        </div>
        <div id="deletewindow">
            <div>
                Eliminar archivo
            </div>
            <div>
                <div>
                    Atencion!! todos los cambios y eliminacion de archivos son monitoreados y se lleva un historial de cada evento
                <br />
                    Para continuar proporcione su clave de acceso a Dashboard
                </div>
                <div>
                    <input id="jqxPasswordInput" type="password" style="margin-top: 5px; align-content: center;" />
                    <div style="margin-top: 15px;">
                        <input type="button" id="cmddelete" value="Eliminar" style="margin-right: 10px" />
                        <input type="button" id="cmdcanceldelete" value="Cancelar" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div id='Menu' style="display: none">
        <ul>
            <li>Abrir archivo</li>
        </ul>
    </div>
    <input type="hidden" id="cmdsendsingle" />
    <input type="hidden" id="cmdsendsinglefull" />
    <input type="hidden" id="cmddeletesingle" />
    <input type="hidden" id="cmddeletesinglefull" />
    <input type="hidden" id="ctrlref" />
    <input type="hidden" id="ctrlfacturas" />
    <input type="hidden" id="ctrlconsolidados" />
    <input type="hidden" id="ctrlpreentrada" />
    <input type="hidden" id="cuentaarchivo" />

    <!-- BEGIN JAVASCRIPT -->
    <script src="assets/js/libs/jquery/jquery-1.11.2.min.js"></script>
    <script src="assets/js/libs/jquery/jquery-migrate-1.2.1.min.js"></script>
    <script src="assets/js/libs/bootstrap/bootstrap.min.js"></script>
    <script src="assets/js/libs/spin.js/spin.min.js"></script>
    <script src="assets/js/libs/autosize/jquery.autosize.min.js"></script>
    <script src="assets/js/libs/dropzone/dropzone.min.js"></script>
    <script src="assets/js/libs/underscore/underscore-min.js"></script>
    <script src="assets/js/libs/nanoscroller/jquery.nanoscroller.min.js"></script>
    <script src="assets/js/libs/jquery-validation/dist/jquery.validate.min.js"></script>
    <script src="assets/js/libs/jquery-validation/dist/additional-methods.min.js"></script>
    <script src="assets/js/core/source/App.js"></script>
    <script src="assets/js/core/source/AppNavigation.js"></script>
    <script src="assets/js/core/source/AppOffcanvas.js"></script>
    <script src="assets/js/core/source/AppCard.js"></script>
    <script src="assets/js/core/source/AppForm.js"></script>
    <script src="assets/js/core/source/AppNavSearch.js"></script>
    <script src="assets/js/core/source/AppVendor.js"></script>
    <script src="assets/js/libs/autosize/jquery.autosize.min.js"></script>
    <script src="assets/js/libs/jcookie/jquery.cookie.js"></script>
    <script type="text/javascript" src="assets/js/libs/jqwidgets/jqx-all.js"></script>

    <script src="assets/js/core/demo/SessionExtender.js"></script>
    <script src="assets/js/libs/bootbox/bootbox.min.js"></script>
    <script src="assets/js/core/demo/demo.js"></script>
    <script src="assets/js/core/demo/expedientedigital.js"></script>
    <!-- END JAVASCRIPT -->
</body>
</html>


