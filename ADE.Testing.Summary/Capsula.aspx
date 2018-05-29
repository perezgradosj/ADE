<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Capsula.aspx.cs" Inherits="ADE.Testing.Summary.Capsula" ValidateRequest="false"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        Ingrese Tipo de Resumen: <input type="text" runat="server" id="td"/><br />
        Ingrese la fecha del resumen: <input type="text" runat="server" id="fecha"/><br />
        Ingrese el RUC de la empresa:<input type="text" runat="server" id="ruc"/><br />
        <br />
        <asp:Button id="envia" runat="server" OnClick="envia_Click" Text="Consulta weon!" />
    </div>
    </form>
</body>
</html>
