using ADE.Configurations.DataAccess;
using ADE.Configurations.Entities;
using ADE.Configurations.Objects;
using System;

namespace ADE.Processes.DatabaseIncome
{
    public class DatabaseIncome
    {
        public MainSettings MS = null;
        public Validation VD = null;
        DatabaseConnection DBConnection = null;
        public DatabaseIncome(MainSettings MSI)
        {
            MS = MSI;
            DBConnection = new DatabaseConnection(MS);
        }

        public bool InsertInDatabase(DBDocument Document, Extras.Common.Method.ListUtilClass List, string TypeRC)
        {
            bool insert = false;
            int IDC = 0;
            MS.GeneraLog.IngresoBD01("---------- INICIO DE INGRESO A BD ----------");
            MS.GeneraLog.IngresoBD01("Nombre del Archivo a guardar: " + MS.FileLocation);

            if (!MS.DocumentType.StartsWith("R"))
            {
                if (DBConnection.SP_ValidarDocumentoExiste(MS.DocumentName))
                {
                    MS.GeneraLog.IngresoBD01("Documento " + MS.DocumentName + " ya fue ingresado a la Base de Datos");
                    MS.GeneraLog.IngresoBD01("---------- FIN   DE INGRESO A BD ----------");
                    MS.GeneraLog.RegistraError(MS.FileName);
                    return false;
                }
                try
                {
                    IDC = DBConnection.SP_InsertaCabeceraDocumento(Document.Cabecera);
                    if (IDC > 0)
                    {
                        string IdDetail = DBConnection.SP_InsertaDetalleDocumento(Document, IDC);
                        if (IdDetail != "1")
                        {
                            insert = false;
                            MS.GeneraLog.RegistraError(MS.FileName);
                        }
                        else
                        {
                            insert = true;
                        }
                    }
                    else
                    {
                        insert = false;
                        MS.GeneraLog.RegistraError(MS.FileName);
                    }
                }
                catch (Exception e)
                {
                    insert = false;
                    MS.GeneraLog.IngresoBD01("Error al ingresar a Base de Datos: " + e.Message);
                    MS.GeneraLog.RegistraError(MS.FileName);
                }
            }
            else
            {
                if(MS.DocumentType == "RC")
                {
                    if(Document.RBD.Count > 0)
                    {
                        Document.RBC.NUM_CPE = MS.DocumentName;
                        Document.RBC.NUM_SEC = MS.SummaryNumber;

                        //IDC = DBConnection.SP_InsertaCabeceraRC(Document.RBC, List);

                        

                        IDC = DBConnection.SP_InsertaCabeceraRC(Document.RBC, List, TypeRC); //nuevo resumen

                        Document.RBC.ID_RBC = IDC;
                        if (IDC > 0)
                        {
                            //DBConnection.SP_InsertaDetalleRC(IDC, Document.RBD); comment
                            //DBConnection.SP_InsertaDetalleRC(IDC, Document.RBD); ya no inserta detalle
                            insert = true;
                        }
                    }
                }

                if (MS.DocumentType == "RA" || MS.DocumentType == "RR")
                {
                    if (Document.RAD.Count > 0)
                    {
                        Document.RAC.NUM_CPE = MS.DocumentName;
                        Document.RAC.NUM_SEC = MS.SummaryNumber;

                        IDC = DBConnection.SP_InsertaCabeceraRA(Document.RAC);
                        Document.RAC.ID_RAC = IDC;
                        if (IDC > 0)
                        {
                            DBConnection.SP_InsertaDetalleRA(Document.RAD, Document.RAC);
                            insert = true;
                        }
                    }
                }
            }
            
            MS.GeneraLog.IngresoBD01("---------- FIN   DE INGRESO A BD ----------: " + insert);
            return insert;
        }
    }
}

