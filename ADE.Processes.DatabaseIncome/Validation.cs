using ADE.Configurations.DataAccess;
using ADE.Configurations.Entities;
using ADE.Configurations.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ADE.Processes.DatabaseIncome
{
    public class Validation
    {
        public MainSettings MS = null;
        public List<int> Taxes = new List<int>();
        public Dictionary<string, string> Interface = new Dictionary<string, string>();
        public int it, li, re, an, da, ex;
        public string DocType = "";

        public Validation(MainSettings MSI)
        {
            MS = MSI;
        }

        public bool ValidateInterface(List<string> DataTXT)
        {
            bool valido = true;
            int i = 0, j = 0;
            List<RegexDB> Validaciones = new List<RegexDB>();
            List<string> Errores = new List<string>();
            List<string> Duplicates = new List<string>();
            DatabaseConnection DBConnection = new DatabaseConnection(MS);
            Dictionary<string, string[]> DataDict = new Dictionary<string, string[]>();

            MS.GeneraLog.Validaciones("= = = = INICIO DE VALIDACIONES = = = =");
            MS.GeneraLog.Validaciones("Nombre del Archivo a validar: " + MS.FileLocation);
            it = li = re = an = da = 0;

            Validaciones = DBConnection.SP_ObtieneValidaciones();
            if (Validaciones.Count == 0)
            {
                MS.GeneraLog.Validaciones("No se han podido obtener la validaciones de la base de datos");
                MS.GeneraLog.Validaciones("= = = = FIN    DE VALIDACIONES = = = =");
                return false;
            }
            
            foreach (string line in DataTXT)
            {
                string[] temx = line.Split('|');
                string[] temp = getSizedTmp(temx, Validaciones);
                if (temp[0].StartsWith("DOCUMENTO-AFECTADO") || temp[0].StartsWith("ITEM") || temp[0].StartsWith("LINEA") || temp[0].StartsWith("REFERENCIA") || temp[0].StartsWith("ANTICIPO") || temp[0].StartsWith("EXTRAS"))
                {
                    if (DataDict.ContainsKey(temp[0] + temp[1]))
                    {
                        Errores.Add($" [error] Linea {i.ToString().PadLeft(3, '0')}, Posicion 2.[{temp[0]} - {temp[1]}]  ya existe en una línea anterior");
                    }
                    else
                        DataDict.Add(temp[0] + temp[1], temp);

                    if (temp[0].CompareTo("LINEA") == 0) li++;
                    if (temp[0].CompareTo("ITEM") == 0) it++;
                    if (temp[0].CompareTo("DOCUMENTO-AFECTADO") == 0) da++;
                    if (temp[0].CompareTo("REFERENCIA") == 0) re++;
                    if (temp[0].CompareTo("ANTICIPO") == 0) an++;
                    if (temp[0].CompareTo("EXTRAS") == 0) ex++;
                }
                if(temp[0] == "CABECERA-PRINCIPAL")
                {
                    DocType = temp[1];
                }
                if (temp[0] == "TOTAL")
                {
                    double o = 0;
                    if ("07|08".Contains(DocType))
                    {
                        if (double.TryParse(temp[4], out o))
                            if (o > 0) Taxes.Add(4);
                        
                        if (double.TryParse(temp[5], out o))
                            if (o > 0) Taxes.Add(5);

                        if (double.TryParse(temp[6], out o))
                            if (o > 0) Taxes.Add(6);
                    }
                    else
                    {
                        if (double.TryParse(temp[6], out o))
                            if (o > 0) Taxes.Add(6);

                        if (double.TryParse(temp[7], out o))
                            if (o > 0) Taxes.Add(7);

                        if (double.TryParse(temp[8], out o))
                            if (o > 0) Taxes.Add(8);
                    }
                    
                }
            }

            for (j = 0; j < Validaciones.Count; ++j)
            {
                Validaciones[j]._POS = j;
                Validaciones[j]._USE = 0;
                Interface[Validaciones[j].NOM] = "";

                //Interface["asmfklansf"] = "";
            }

            foreach (string line in DataTXT)
            {
                i++;
                string[] temx = line.Split('|');
                string[] temp = getSizedTmp(temx, Validaciones);

                try
                {
                    string tab = temp[0];

                    for (j = 0; j < (temp.Length - 1); ++j)
                    {
                        int k = j + 1;
                        string key = tab + "-" + k;
                        List<RegexDB> val = new List<RegexDB>();
                        val = Validaciones.Where(o => o.KEY == key && o.DOC.Contains(MS.DocumentType)).ToList();
                        temp[k] = temp[k].Trim();
                        if (val.Count > 0)
                        {
                            if (val[0].DOC.Contains(MS.DocumentType))
                            {
                                Regex r = new Regex(@"" + val[0].VAL, RegexOptions.IgnoreCase);
                                Match m = r.Match(temp[k]);
                                Validaciones[val[0]._POS]._USE = 1;
                                if (m.Success)
                                {
                                    if (temp[0].StartsWith("DOCUMENTO-AFECTADO") || temp[0].StartsWith("ITEM") || temp[0].StartsWith("LINEA") || temp[0].StartsWith("REFERENCIA") || temp[0].StartsWith("ANTICIPO") || temp[0].StartsWith("EXTRAS"))
                                    {
                                        //if (Interface.ContainsKey(val[0].NOM + temp[1]))
                                        //{
                                        //    Errores.Add($" [error] Linea {i.ToString().PadLeft(3, '0')}, Posicion {k.ToString().PadLeft(2, '0')} ha sido ingresado mas de una vez");
                                        //}
                                        //else
                                        //{
                                            Interface[val[0].NOM + temp[1]] = temp[k];
                                        //}
                                    }
                                    else
                                    {
                                        Interface[val[0].NOM] = temp[k];
                                    }
                                }
                                else
                                {
                                    if (val[0].MND == "S")
                                    {
                                        if (temp[k] != "")
                                        {
                                            Errores.Add($" [error] Linea {i.ToString().PadLeft(3, '0')}, Posicion {k.ToString().PadLeft(2, '0')} [" + val[0].NOM + "] " + val[0].MSG);
                                        }
                                        else
                                        {
                                            Errores.Add($" [error] Linea {i.ToString().PadLeft(3, '0')}, Posicion {k.ToString().PadLeft(2, '0')} [" + val[0].NOM + "] " + val[0].MSG);
                                        }
                                    }
                                    else
                                    {
                                        if (temp[k] != "")
                                        {
                                            Errores.Add($" [error] Linea {i.ToString().PadLeft(3, '0')}, Posicion {k.ToString().PadLeft(2, '0')} [" + val[0].NOM + "] " + val[0].MSG);
                                        }
                                        else
                                        {
                                            if (temp[0].StartsWith("DOCUMENTO-AFECTADO") || temp[0].StartsWith("ITEM") || temp[0].StartsWith("LINEA") || temp[0].StartsWith("REFERENCIA") || temp[0].StartsWith("ANTICIPO") || temp[0].StartsWith("EXTRAS"))
                                            {
                                                Interface[val[0].NOM + temp[1]] = "";
                                            }
                                            else
                                            {
                                                Interface[val[0].NOM] = "";
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (temp[j + 1] != "")
                                {
                                    MS.GeneraLog.Validaciones(" [aviso] [" + val[0].NOM + "][" + val[0].KEY + "] Este campo no es necesario para el Documento ");
                                }
                            }
                        }
                        else
                        {
                            MS.GeneraLog.Validaciones($" [aviso] Linea:{i} => [{tab}->pos: {k}]  Este campo no tiene reglas de validación");
                        }
                    }
                }
                catch (Exception ValidacionExcepcion)
                {

                }
            }

            /// Validacion momentanea, para retenciones
            for (int s = 0; s < it; ++s)
            {
                int q = s + 1;
                if (DataDict.ContainsKey("ITEM" + (s + 1)) && MS.DocumentType == "20")
                {
                    if (DataDict["ITEM" + (s + 1)][6] != "PEN")
                    {
                        if (DataDict["ITEM" + (s + 1)][16] == "" || DataDict["ITEM" + (s + 1)][17] == "" || DataDict["ITEM" + (s + 1)][18] == "" || DataDict["ITEM" + (s + 1)][19] == "")
                        {
                            Errores.Add($" [error] ITEM {(s + 1).ToString().PadLeft(3, '0')} Los datos acerca del Tipo de Cambio son obligatorios");
                        }
                    }
                    DateTime DEmi, DPag;
                    if (DateTime.TryParse(Interface["FEmisDocRelac" + q],out DEmi) && DateTime.TryParse(Interface["FecMovi" + q], out DPag))
                    {
                        if(DPag < DEmi)
                        {
                            Errores.Add($" [error] ITEM {(s + 1).ToString().PadLeft(3, '0')} La fecha de pago debe estar entre el primer día calendario del mes al cual corresponde la fecha de emisión del comprobante de retención o desde la fecha de emisión del comprobante relacionado.");
                        }
                    }
                }
            }



            // Armamos el nombre del Documento
            Interface["NUM_CE"] = $"{Interface["EmiNumDocu"]}-{Interface["TipoCE"]}-{Interface["Id_CE"]}";

            // En caso existan errores, se registran en el LOG
            if (Errores.Count > 0)
            {
                valido = false;
                foreach (string valid in Errores)
                {
                    MS.GeneraLog.Validaciones(valid);
                }
            }

            // Verificamos que no hayan campos faltantes en la interface
            List<RegexDB> emp = new List<RegexDB>();
            emp = Validaciones.Where(o => o._USE == 0 && o.DOC.Contains(MS.DocumentType) && o.MND == "S").ToList();
            if (emp.Count > 0)
            {
                foreach (RegexDB valid in emp)
                {
                    MS.GeneraLog.Validaciones($"[error] Tag {valid.TAB} - Campo {valid.NOM} es obligatorio");
                }
                valido = false;
            }

            // Verificamos que no hayan detracciones imcompletas
            List<RegexDB> detF = new List<RegexDB>();
            detF = Validaciones.Where(o => o._USE == 1 && o.DOC.Contains(MS.DocumentType) && o.TAB == "DETRACCION").ToList();
            
            if (detF.Count > 0)
            {
                foreach (RegexDB D in detF)
                {
                    if (Interface.ContainsKey(D.NOM))
                    {
                        if (Interface[D.NOM].Trim() == "")
                        {
                            MS.GeneraLog.Validaciones($"[error] Tag {D.TAB} - Campo {D.NOM} es obligatorio");
                            valido = false;
                        }
                    }
                    else
                    {
                        MS.GeneraLog.Validaciones($"[error] Tag {D.TAB} - Campo {D.NOM} es obligatorio");
                        valido = false;
                    }
                }
            }


            if (valido)
            {
                MS.GeneraLog.Validaciones("Todos los datos son correctos");
            }

            MS.GeneraLog.Validaciones("= = = = FIN    DE VALIDACIONES = = = =");
            MS.DocumentName = Interface["NUM_CE"];
            return valido;
        }

        private string[] getSizedTmp(string[] tmp, List<RegexDB> Validaciones)
        {
            int nlines;
            nlines = Validaciones.Where(o => o.TAB == tmp[0] && o.DOC.Contains(MS.DocumentType)).Count() + 1;
            List<string> mod = tmp.OfType<string>().ToList();
            if (mod.Count < nlines)
            {
                int a = nlines - mod.Count;
                for (int i = 0; i < a; ++i)
                {
                    mod.Add("");
                }
            }
            string[] res = mod.ToArray();
            return res;
        }
    }
}
