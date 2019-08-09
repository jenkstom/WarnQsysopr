using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dblib
{
    //classes DBConnection, DBManager

    public class DBConnection
    {
        public String conntype;
        public bool eof = false;
        public bool inUse = false;
        public int fieldCount = 0;

        private object conn;
        private object cmd;
        private object rdr;
        private String UserID = "";
        private String Password = "";
        private String as400 = "10.0.2.1";
        private String tmt = "10.0.2.20";
        private String tmtUser = "sa";
        private String tmtPass = "sapassword";

        public DBConnection(String dbtype = "400")
        {
            conntype = dbtype.ToUpper();

            switch (dbtype)
            {
                case "400":
                    conn = new ADODB.Connection();
                    (conn as ADODB.Connection).Open($"Driver=IBM i Access ODBC Driver;QueryTimeOut=0;System={as400};Uid={UserID};Pwd={Password};Translate Binary=true;",
                            UserID, Password, 0);
                    rdr = new ADODB.Recordset();
                    (rdr as ADODB.Recordset).CursorLocation = ADODB.CursorLocationEnum.adUseClient;
                    break;
                case "TMT":
                    conn = new SqlConnection($"server={tmt};user id={tmtUser};password={tmtPass};initial catalog=TMWAMS");
                    break;
            }
            inUse = true;
        }

        public bool RunSql(String sql)
        {
            bool result = true;
            eof = false;
            sql = sql.Replace("\n", " ").Replace("\r", "").Replace("\t", "").Trim();
            switch (conntype)
            {
                case "400":
                    if ((conn as ADODB.Connection).State == 1)
                        (conn as ADODB.Connection).Close();
                    if ((conn as ADODB.Connection).State == 0)
                        (conn as ADODB.Connection).Open();
                    (rdr as ADODB.Recordset).Open(sql, (conn as ADODB.Connection),
                        ADODB.CursorTypeEnum.adOpenForwardOnly, ADODB.LockTypeEnum.adLockReadOnly, -1);
                    fieldCount = (rdr as ADODB.Recordset).Fields.Count;
                    eof = (rdr as ADODB.Recordset).EOF;
                    break;
                default:
                    if ((conn as SqlConnection).State == ConnectionState.Open)
                        (conn as SqlConnection).Close();
                    if ((conn as SqlConnection).State == ConnectionState.Closed)
                        (conn as SqlConnection).Open();
                    cmd = new SqlCommand(sql, (conn as SqlConnection));
                    rdr = (cmd as SqlCommand).ExecuteReader();
                    fieldCount = (rdr as SqlDataReader).FieldCount;
                    if (!(rdr as SqlDataReader).Read())
                        eof = true;
                    break;
            }
            inUse = true;
            return result;
        }

        public bool RunSql(String fmt, params Object[] objs)
        {
            return RunSql(String.Format(fmt, objs));
        }

        public bool ExecSql(String sql)
        {
            bool result = true;
            eof = true;
            fieldCount = 0;
            sql = sql.Replace("\n", " ").Replace("\r", "").Replace("\t", "").Trim();
            switch (conntype)
            {
                case "400":
                    if ((conn as ADODB.Connection).State == 1)
                        (conn as ADODB.Connection).Close();
                    if ((conn as ADODB.Connection).State == 0)
                        (conn as ADODB.Connection).Open();
                    (rdr as ADODB.Recordset).Open(sql, (conn as ADODB.Connection),
                        ADODB.CursorTypeEnum.adOpenForwardOnly, ADODB.LockTypeEnum.adLockReadOnly, -1);
                    break;
                default:
                    if ((conn as SqlConnection).State == ConnectionState.Open)
                        (conn as SqlConnection).Close();
                    if ((conn as SqlConnection).State == ConnectionState.Closed)
                        (conn as SqlConnection).Open();
                    cmd = new SqlCommand(sql, (conn as SqlConnection));
                    rdr = (cmd as SqlCommand).ExecuteReader();
                    break;
            }
            inUse = true;
            return result;
        }

        public bool ExecSql(String fmt, params Object[] objs)
        {
            return ExecSql(String.Format(fmt, objs));
        }

        public void MoveNext()
        {
            switch (conntype)
            {
                case "400":
                    (rdr as ADODB.Recordset).MoveNext();
                    eof = (rdr as ADODB.Recordset).EOF;
                    break;
                default:
                    eof = !(rdr as SqlDataReader).Read();
                    break;
            }
        }

        public void Close()
        {
            if (rdr != null)
                switch (conntype)
                {
                    case "400":
                        //(rdr as ADODB.Recordset).Close();
                        //(conn as JCT.FrameWork.DataAccess.AS400).cn.Close();
                        if ((conn as ADODB.Connection).State == 1)
                            (conn as ADODB.Connection).Close();

                        break;
                    default:
                        if (!(rdr as SqlDataReader).IsClosed)
                            (rdr as SqlDataReader).Close();
                        rdr = null;
                        cmd = null;
                        break;
                }
            eof = true;
            inUse = false;
            fieldCount = 0;
        }

        public String stringValue(int i)
        {
            String result = "";
            switch (conntype)
            {
                case "400":
                    result = (rdr as ADODB.Recordset).Fields[i].Value.ToString().Trim();
                    break;
                default:
                    result = (rdr as SqlDataReader)[i].ToString().Trim();
                    break;
            }
            return result;
        }

        public String stringValue(String fieldname)
        {
            String result = "";
            switch (conntype)
            {
                case "400":
                    result = (rdr as ADODB.Recordset).Fields[fieldname].Value.ToString().Trim();
                    break;
                default:
                    result = (rdr as SqlDataReader)[fieldname].ToString().Trim();
                    break;
            }
            return result;
        }

        public object Value(int i)
        {
            object result = "";
            switch (conntype)
            {
                case "400":
                    result = (rdr as ADODB.Recordset).Fields[i].Value;
                    break;
                default:
                    result = (rdr as SqlDataReader)[i];
                    break;
            }
            return result;
        }

        public object Value(String fieldname)
        {
            object result = "";
            switch (conntype)
            {
                case "400":
                    result = (rdr as ADODB.Recordset).Fields[fieldname].Value;
                    break;
                default:
                    result = (rdr as SqlDataReader)[fieldname];
                    break;
            }
            return result;
        }

        public String fieldName(int i)
        {
            String result = "";
            switch (conntype)
            {
                case "400":
                    result = (rdr as ADODB.Recordset).Fields[i].Name;
                    break;
                default:
                    result = (rdr as SqlDataReader).GetName(i);
                    break;
            }
            return result;
        }

    }

    public class DBManager
    {
        private List<DBConnection> DBConnections = new List<DBConnection>();

        ~DBManager()
        {
            destroyAllConns();
        }

        public void destroyAllConns()
        {
            foreach (DBConnection conn in DBConnections)
            {
                if (conn.inUse)
                    try
                    {
                        conn.Close();
                    }
                    catch { }
            }
        }

        public DBConnection getConnObject(String ctype = "400")
        {
            ctype = ctype.ToUpper();
            //go through connections and find one of the appropriate type that is not in use
            foreach (DBConnection conn in DBConnections)
            {
                if (conn.conntype == ctype && !conn.inUse)
                {
                    return conn;
                }
            }

            DBConnection newconn = new DBConnection(ctype);
            DBConnections.Add(newconn);
            //if we can't find one, then allocate one and add it to the dbConns list
            return newconn;
        }

        public String parmClean(String x)
        {
            return x.Replace("''","'").Replace("'", "''");

        }
    }
}
