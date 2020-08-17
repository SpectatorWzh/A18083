using Agv_Info;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace AGVSystem
{
    public class Cls_A18083
    {

        //static AgvLib.HxAgv[] agvBtn;  

        static Thread[] th_getData;
        public static DataSet ds_mileage = new DataSet();
        public DataSet ds_Mileage
        {
            get { return ds_mileage; }
            set { ds_mileage = value; }
        }

        public static DataSet ds_aGVGo = new DataSet(); //AGV放行信息
        public DataSet ds_AGVGo
        {
            get { return ds_aGVGo; }
            set { ds_aGVGo = value; }
        }

        public static int[] agvRecConnCount;

        static byte agv_Qty = 0;
        public byte Agv_Qty
        {
            get { return agv_Qty; }
            set { agv_Qty = value; }
        }

        static byte[] go_Flag = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                 0,0,0,0,0};

        public byte[] Go_Flag
        {
            get { return go_Flag; }
            set { }
        }

        static byte[] card_input = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                     0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                     0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public byte[] Card_Input
        {
            get { return card_input; }
            set { card_input = value; }
        }


        public int[] AgvRecConnCount
        {
            get { return agvRecConnCount; }
            set { agvRecConnCount = value; }
        }

        static byte[] card_output = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public byte[] Card_Output
        {
            get { return card_output; }
            set { card_output = value; }
        }

        static string connectStr = "";
        public string ConnectStr
        {
            get { return connectStr; }
            set { connectStr = value; }
        }

        static byte[] agv_status = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                    0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                    0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                    0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                    0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                    0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                    0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                    0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                    0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                    0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                    0,0,0,0,0};
        /// <summary>
        /// agv状态：1-运行中  2-定位中  3-未启动
        /// </summary>
        public byte[] Agv_Status
        {
            get { return agv_status; }
            set { agv_status = value; }
        }

        static byte[] agv_addr_end = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                      0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                      0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                      0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                      0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                      0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                      0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                      0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                      0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                      0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                      0,0,0,0,0};
        /// <summary>
        /// agv终点目标地址
        /// </summary>
        public byte[] Agv_Addr_end
        {
            get { return agv_addr_end; }
            set { agv_addr_end = value; }
        }

        static bool startFlag = false;
        /// <summary>
        /// AGV调度启动标志
        /// </summary>
        public bool StartFlag
        {
            get { return startFlag; }
            set { startFlag = value; }
        }

        static int closedFlag = -1;
        /// <summary>
        /// AGV调度关闭标志
        /// </summary>
        public int ClosedFlag
        {
            get { return closedFlag; }
            set { closedFlag = value; }
        }

        /// <summary>
        /// plc型号
        /// </summary>
        static byte plc_type = 0;
        public byte Plc_Type
        {
            get { return plc_type; }
            set { plc_type = value; }
        }

        static byte buffer1 = 0;
        /// <summary>
        /// 缓存区1计数
        /// </summary>
        public byte Buffer1
        {
            get { return buffer1; }
            set { buffer1 = value; }
        }
        static byte buffer2 = 0;
        /// <summary>
        /// 缓存区2计数
        /// </summary>
        public byte Buffer2
        {
            get { return buffer2; }
            set { buffer2 = value; }
        }
        static byte buffer3 = 0;
        /// <summary>
        /// 缓存区3计数
        /// </summary>
        public byte Buffer3
        {
            get { return buffer3; }
            set { buffer3 = value; }
        }

        /// <summary>
        /// AGV报警信息
        /// </summary>
        static int[] agvAlarmTemp = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                     0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                     0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                     0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                     0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                     0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                     0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                     0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                     0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                     0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                     0,0,0,0,0};

        /// <summary>
        /// agv 位置信息
        /// </summary>
        static int[] agvPosTemp ={0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                  0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                  0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                  0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                  0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                  0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                  0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                  0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                  0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                  0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                  0,0,0,0,0};  //
        /// <summary>
        /// agv 位置信息
        /// </summary>
        public int[] AgvPosTemp
        {
            get { return agvPosTemp; }
            set { agvPosTemp = value; }
        }



        public Thread[] th_GetData
        {
            get { return th_getData; }
            set { th_getData = value; }
        }

        /// <summary>
        /// AGV路口放行逻辑表
        /// </summary>
        static System.Data.DataSet ds_crossInfo = new System.Data.DataSet();
        /// <summary>
        /// AGV路口放行逻辑表
        /// </summary>
        public System.Data.DataSet ds_CrossInfo
        {
            get { return ds_crossInfo; }
            set { ds_crossInfo = value; }
        }

        /// <summary>
        /// AGV路口复位逻辑表
        /// </summary>
        static DataSet ds_reset_relation = new DataSet();
        /// <summary>
        /// AGV路口复位逻辑表
        /// </summary>
        public DataSet ds_Reset_Relation
        {
            get { return ds_reset_relation; }
            set { ds_reset_relation = value; }
        }

        public static System.Data.DataSet ds_location = new System.Data.DataSet();//保存所有地标
        public System.Data.DataSet ds_Location
        {
            get { return ds_location; }
            set { ds_location = value; }
        }

        public static System.Data.DataSet ds_aGVWarn = new System.Data.DataSet(); //AGV报警信息
        public System.Data.DataSet ds_AGVWarn
        {
            get { return ds_aGVWarn; }
            set { ds_aGVWarn = value; }
        }

        public static System.Data.DataSet ds_warn = new System.Data.DataSet(); //报警信息
        public System.Data.DataSet ds_Warn
        {
            get { return ds_warn; }
            set { ds_warn = value; }
        }

        public static bool maintain_st = false;
        public bool Maintain
        {
            get { return maintain_st; }
            set { maintain_st = value; }
        }

        public void AGV_start(object agvinfo)
        {
            agv newagv = new agv();
            Cls.AgvInfo temp = (Cls.AgvInfo)agvinfo;
            th_GetData[temp.Agv_ID - 1] = new Thread(newagv.AGV_Data);
            th_GetData[temp.Agv_ID - 1].Start(agvinfo);
        }

        Thread th;
        public void Function()
        {
        }

        public void Thread_Closed()
        {
            th.Abort();
        }



        public class agv
        {
            public void AGV_Data(object agvinfo)
            {
                #region 常规代码
                Cls.AgvInfo temp = (Cls.AgvInfo)agvinfo;
                int location_temp = 0, status_temp = 0, warn_temp = 0, path_temp = 0;
                int location_value = 0, warn_value = 0, path_value = 0;
                byte[] param_info = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                int t = 0;
                Ping pingAgv = new Ping();
                int errCount = 0;
                int sendcount = 0;
                Soc_Client Link_To_PLC = new Soc_Client(temp.IP, 4001);
                temp.Soc = Link_To_PLC.clientSocket;
                string warn_info = "";
                int warn_level = 0;
                int last_landmark = 0;
                closedFlag = -1;
                //test

                //end test


                while (true)
                {
                    if (closedFlag > 0)
                    {
                        closedFlag--;
                        return;
                    }
                    if (!startFlag)
                    {
                        Thread.Sleep(500);
                        continue;
                    }
                    try
                    {
                        if (pingAgv.Send(temp.IP, 150).Status == IPStatus.Success)
                        {
                            //来到这里，表示PING成功。
                            if (!temp.bPlcLinked)
                            {
                                if (Link_To_PLC.OpenLinkPLC())
                                {   //来到这里，表示链路成功打开
                                    temp.bPlcLinked = true;
                                }
                                else
                                {   //来到这里，表示要重新建立链路
                                    temp.bPlcLinked = false;
                                    Link_To_PLC.CloseLinkPLC();
                                    Link_To_PLC = new Soc_Client(temp.IP, 4001);
                                    temp.Soc = Link_To_PLC.clientSocket;
                                    LostConnect(ref agvRecConnCount[temp.Agv_ID - 1]);
                                }
                            }
                            else
                            {
                                //来到这里，表示PLC的链路已经打开，可以直接读写数据
                                if (0 != Link_To_PLC.ReadInfo(out param_info))
                                {
                                    //来到这里，表示读数据失败    
                                    if (agvRecConnCount[temp.Agv_ID - 1] >= 0)
                                    {
                                        agvRecConnCount[temp.Agv_ID - 1] = errCount;
                                    }
                                    else
                                    {
                                        LostConnect(ref agvRecConnCount[temp.Agv_ID - 1]);
                                    }

                                    if (Math.Abs(agvRecConnCount[temp.Agv_ID - 1]) >= 5)
                                    {
                                        //来到这里，表示一再失败，要重新建立链路                                    
                                        temp.bPlcLinked = false;
                                        agvRecConnCount[temp.Agv_ID - 1] = 0;
                                        Link_To_PLC.CloseLinkPLC();
                                        Link_To_PLC = new Soc_Client(temp.IP, 4001);
                                        temp.Soc = Link_To_PLC.clientSocket;
                                        temp.Warn_ID = "-1";
                                        Command("INSERT INTO [Net_Log] ([dt] ,[AGV_ID] ,[Type]) VALUES  ('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "','" + temp.Agv_ID + "','Read Fail')");
                                    }
                                }
                                else
                                {
                                    if (Math.Abs(agvRecConnCount[temp.Agv_ID - 1]) > 0)
                                    {
                                        agvRecConnCount[temp.Agv_ID - 1] = 0;
                                    }
                                    //读位置
                                    //Func.GetLandMarkId(temp.AssemblyLine, param_info[3] * 256 + param_info[2]);
                                    location_temp = GetLandMarkId(temp.AssemblyLine, param_info[3] * 256 + param_info[2]);
                                    //保存地标值
                                    if (temp.Location_Display != location_temp)
                                    {
                                        //记录充电
                                        if (temp.Location_Display == 106)
                                        {
                                            Command("INSERT INTO [Change_Log]([Agv_ID] ,[Start_time],[Finish_time] ,[Location_ID])  VALUES(" +
                                                temp.Agv_ID + ",'" + temp.Read_dt + "','" + DateTime.Now.ToString() + "'," + temp.Location_Display + ")");
                                        }

                                        //记录里程
                                        temp.Mileage = temp.Mileage + GetMileage(temp.Location_Display, location_value);
                                        temp.Read_dt = DateTime.Now;
                                        last_landmark = temp.Location_Display;
                                        temp.Location_Display = location_temp;
                                        agvPosTemp[temp.Agv_ID - 1] = location_temp;

                                        Command("INSERT INTO [Location_Log] ([Dt] ,[Agv_ID] ,[Loc_ID] ,[Value] ,[Loc_ID2]  ,[Value2],[Value3])  VALUES ('" +
                                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'," + temp.Agv_ID + "," + location_temp + "," + location_value + "," + path_temp + "," + path_value + "," + status_temp + ")");
                                    }
                                    //设备状态信息
                                    if (warn_temp != warn_value || temp.Warn_ID == "-1")
                                    {
                                        warn_temp = warn_value;
                                        if (warn_value == 1)//正常
                                        {
                                            WarnInfo(11, out warn_info, out warn_level);
                                            Agv_Alarm_Insert(temp.Agv_ID, temp.AssemblyName, temp.AssemblyLine.ToString(), temp.Internal_ID, warn_info, temp.Agv_ID);
                                            temp.Warn_ID = "11";
                                        }
                                        if ((warn_value & 8) == 8)//障碍物
                                        {
                                            WarnInfo(3, out warn_info, out warn_level);
                                            Agv_Alarm_Insert(0, temp.AssemblyName, temp.AssemblyLine.ToString(), temp.Internal_ID, warn_info, temp.Agv_ID);
                                            temp.Warn_ID = "3";
                                        }

                                        if ((warn_value & 2) == 2)//低电压异常
                                        {
                                            WarnInfo(10, out warn_info, out warn_level);
                                            Agv_Alarm_Insert(0, temp.AssemblyName, temp.AssemblyLine.ToString(), temp.Internal_ID, warn_info, temp.Agv_ID);
                                            temp.Warn_ID = "10";
                                        }

                                        if ((warn_value & 4) == 4)//防撞
                                        {
                                            WarnInfo(2, out warn_info, out warn_level);
                                            Agv_Alarm_Insert(0, temp.AssemblyName, temp.AssemblyLine.ToString(), temp.Internal_ID, warn_info, temp.Agv_ID);
                                            temp.Warn_ID = "2";
                                        }

                                        if ((warn_value & 32) == 32)//急停
                                        {
                                            WarnInfo(1, out warn_info, out warn_level);
                                            Agv_Alarm_Insert(0, temp.AssemblyName, temp.AssemblyLine.ToString(), temp.Internal_ID, warn_info, temp.Agv_ID);
                                            temp.Warn_ID = "1";
                                        }

                                        if ((warn_value & 16) == 16)//脱轨
                                        {
                                            WarnInfo(7, out warn_info, out warn_level);
                                            Agv_Alarm_Insert(0, temp.AssemblyName, temp.AssemblyLine.ToString(), temp.Internal_ID, warn_info, temp.Agv_ID);
                                            temp.Warn_ID = "7";
                                        }
                                    }
                                    Thread.Sleep(500);
                                    #endregion

                                    

                                    ClsDBConn_sql SQL = new AGVSystem.ClsDBConn_sql();
                                    StringBuilder str = new StringBuilder();
                                    #region 一号小车任务查询
                                    
                                    int num = 0;
                                    //一号AGV小车任务分配查询
                                    if (temp.Agv_ID == 1)
                                    {
                                        if (temp.Task_ID == 0)
                                        {
                                            DataSet dt = SQL.connDt(@"select TOP(1) number from [dbo].[LineBodyOne] where isdel=1 order by number");
                                            if (dt.Tables[0].Rows.Count > 0)
                                            {
                                                temp.Task_ID = Convert.ToUInt16(dt.Tables[0].Rows[0]["number"].ToString());
                                                SQL.Command(@"update LineBodyOne set isdel = '0' where number=" + location_temp);
                                            }
                                        }
                                    }
                                    //二号AGV小车任务分配查询
                                    if (temp.Agv_ID == 2)
                                    {
                                        if (temp.Task_ID == 0)
                                        {
                                            int numb = 0;
                                            DataSet d = SQL.connDt(@"select TOP(1) number from [dbo].[LineBodyTwo] where isdel=1  order by number");
                                            if (d.Tables[0].Rows.Count > 0)
                                            {
                                                temp.Task_ID = Convert.ToUInt16(d.Tables[0].Rows[0]["number"].ToString());
                                                SQL.Command(@"update LineBodyTwo set isdel = '0' where number=" + location_temp);
                                            }
                                        }
                                    }
                                    if (temp.Task_ID != temp.Task_Temp)
                                    {
                                        if (Link_To_PLC.WriteInfo(1,temp.Task_ID, 0, out param_info) == 0)
                                        {
                                            CrossInfo_Insert(temp.AssemblyName, temp.AssemblyLine.ToString(), temp.Internal_ID, location_temp, "工位到达", temp.Agv_ID, location_value);
                                            temp.Task_Temp = temp.Task_ID;
                                        }
                                    }

                                    if (temp.Task_ID == location_temp && temp.Task_ID > 0)
                                    {
                                        if (temp.Agv_ID == 1)
                                        {
                                            SQL.Command(@"delete LineBodyOne where isdel = '0' and number=" + location_temp);
                                            temp.Task_ID = 0;
                                            temp.Task_Temp = 0;
                                        }
                                        else if (temp.Agv_ID == 2)
                                        {
                                            SQL.Command(@"delete LineBodyTwo where  isdel = '0' and number=" + location_temp);
                                            temp.Task_ID = 0;
                                            temp.Task_Temp = 0;
                                        }
                                    }
                                    #endregion

                                    //路口放行判断
                                    //一号AGV小车和二号AGV小车在122、114路口放行管控判断
                                    if (location_temp !=temp.Cross_Location_Go)
                                    {
                                        if (location_temp == 122)
                                        {
                                            if (last_landmark == 103 || last_landmark == 114)
                                            {
                                                t = Crossing.Crossing.BlockCross(temp.AssemblyLine, location_temp, temp.Agv_ID, ds_crossInfo);
                                                //路口常规放行
                                                if (t == 1)
                                                {
                                                    if (Link_To_PLC.WriteInfo(1,0, 0, out param_info) == 0)
                                                    {
                                                        CrossInfo_Insert(temp.AssemblyName, temp.AssemblyLine.ToString(), temp.Internal_ID, location_temp, "已放行", temp.Agv_ID, location_value);
                                                        temp.Cross_Location_Go = location_temp;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (Link_To_PLC.WriteInfo(1,0, 0, out param_info) == 0)
                                                {
                                                    CrossInfo_Insert(temp.AssemblyName, temp.AssemblyLine.ToString(), temp.Internal_ID, location_temp, "已放行", temp.Agv_ID, location_value);
                                                    temp.Cross_Location_Go = location_temp;
                                                }
                                            }
                                        }
                                        else if (location_temp == 120)
                                        {
                                            if (last_landmark == 104 || last_landmark == 101)
                                            {
                                                t = Crossing.Crossing.BlockCross(temp.AssemblyLine, location_temp, temp.Agv_ID, ds_crossInfo);
                                                //路口常规放行
                                                if (t == 1)
                                                {
                                                    if (Link_To_PLC.WriteInfo(1, 0, 0, out param_info) == 0)
                                                    {
                                                        CrossInfo_Insert(temp.AssemblyName, temp.AssemblyLine.ToString(), temp.Internal_ID, location_temp, "已放行", temp.Agv_ID, location_value);
                                                        temp.Cross_Location_Go = location_temp;
                                                    }
                                                    else
                                                    {
                                                        int h = 0;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (Link_To_PLC.WriteInfo(1, 0, 0, out param_info) == 0)
                                                {
                                                    CrossInfo_Insert(temp.AssemblyName, temp.AssemblyLine.ToString(), temp.Internal_ID, location_temp, "已放行", temp.Agv_ID, location_value);
                                                    temp.Cross_Location_Go = location_temp;
                                                }
                                                else
                                                {
                                                    int j = 0;
                                                }
                                            }
                                        }
                                        else if(location_temp ==101 || location_temp==103 || location_temp==114 || 
                                            location_temp == 104 || location_temp==116 || location_temp == 102 || location_temp == 100)
                                        {
                                            Crossing.Crossing.BlockCross(temp.AssemblyLine, location_temp, temp.Agv_ID, ds_crossInfo);
                                            CrossInfo_Insert(temp.AssemblyName, temp.AssemblyLine.ToString(), temp.Internal_ID, location_temp, "路口已复位", temp.Agv_ID, location_value);
                                            temp.Cross_Location_Go = location_temp;
                                        }
                                        
                                    }
                                }
                            }
                        }

                        else
                        {
                            if (temp.bPlcLinked)
                            {
                                LostConnect(ref agvRecConnCount[temp.Agv_ID - 1]);
                                if (Math.Abs(agvRecConnCount[temp.Agv_ID - 1]) > 50)
                                {
                                    temp.Warn_ID = "-1";
                                    agvRecConnCount[temp.Agv_ID - 1] = 0;
                                    temp.bPlcLinked = false;
                                    Link_To_PLC.CloseLinkPLC();
                                }
                            }
                        }
                    }
                    catch (Exception Err)
                    {
                        if (Err.TargetSite.Name.ToString() == "ConnectAsync")
                        {
                            temp.Warn_ID = "-1";
                            agvRecConnCount[temp.Agv_ID - 1] = 0;
                            temp.bPlcLinked = false;
                            Link_To_PLC.CloseLinkPLC();
                            Link_To_PLC = new Soc_Client(temp.IP, 4001);
                            temp.Soc = Link_To_PLC.clientSocket;
                        }
                        if (temp.bPlcLinked)
                        {
                            LostConnect(ref agvRecConnCount[temp.Agv_ID - 1]);
                            if (Math.Abs(agvRecConnCount[temp.Agv_ID - 1]) > 50)
                            {
                                temp.Warn_ID = "-1";
                                agvRecConnCount[temp.Agv_ID - 1] = 0;
                                temp.bPlcLinked = false;
                                Link_To_PLC.CloseLinkPLC();
                            }
                        }
                        Thread.Sleep(1000);
                    }
                    Thread.Sleep(1000);

                }
            }

        }//class agv


        private static SqlConnection sqlconn = null;
        private static SqlCommand command = null;
        private static object obj_cmd = new object();
        private static void Command(string sqlstr)
        {
            lock (obj_cmd)
            {
                try
                {
                    if (sqlconn == null)
                    {
                        sqlconn = new SqlConnection(connectStr);
                    }
                    if (sqlconn.State != ConnectionState.Open)
                    {
                        sqlconn.Open();
                    }
                    command = new SqlCommand(sqlstr, sqlconn);
                    command.ExecuteNonQuery();
                    sqlconn.Close();
                }
                catch (Exception err)
                {
                    WriteTxt.SaveLog1("SQL：" + sqlstr + " , \r\n" + err.Message + " \r\n FunctionName：Command", "Command_Log", "系统日志");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="del_agv_no"></param>
        /// <param name="Assembly_Name"></param>
        /// <param name="Assembly_Id"></param>
        /// <param name="AGV_Internal_No"></param>
        /// <param name="AlarmInfo"></param>
        /// <param name="agv_no"></param>
        public static void Agv_Alarm_Insert(int del_agv_no, string Assembly_Name, string Assembly_Id, int AGV_Internal_No, string AlarmInfo, int agv_no)
        {
            lock (obj_agv_alarm_insert)
            {
                agv_alarm_insert(del_agv_no, Assembly_Name, Assembly_Id, AGV_Internal_No, AlarmInfo, agv_no);
            }
        }
        private static object obj_agv_alarm_insert = new object();
        private static void agv_alarm_insert(int del_agv_no, string Assembly_Name, string Assembly_Id, int AGV_Internal_No, string AlarmInfo, int agv_no)
        {
            if (del_agv_no == 0)
            {
                bool update_flag = false;
                if (ds_aGVWarn.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds_aGVWarn.Tables[0].Rows.Count; i++)
                    {
                        if (Convert.ToInt16(ds_aGVWarn.Tables[0].Rows[i][3].ToString()) == agv_no &&
                            ds_aGVWarn.Tables[0].Rows[i][5].ToString().Trim() == AlarmInfo.Trim())
                        {
                            return;
                        }
                    }
                }
                if (!update_flag)
                {
                    DataRow dr = ds_aGVWarn.Tables[0].NewRow();
                    dr[0] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");// 时间 
                    dr[1] = Assembly_Id;   // 线体编号 
                    dr[2] = Assembly_Name;  // 线体名称 
                    dr[3] = agv_no;  // 设备编号 
                    dr[4] = AGV_Internal_No;  // 内部编号
                    dr[5] = AlarmInfo;  // 报警信息
                    ds_aGVWarn.Tables[0].Rows.Add(dr);
                }
            }
            else
            {
                for (int i = 0; i < ds_aGVWarn.Tables[0].Rows.Count; i++)
                {
                    if (AlarmInfo == "正常")
                    {
                        if (Convert.ToInt16(ds_aGVWarn.Tables[0].Rows[i][3]) == del_agv_no)
                        {
                            if (!ds_aGVWarn.Tables[0].Rows[i][5].ToString().Contains("障碍物"))
                            {
                                Command("INSERT INTO [Warn_Log] ([Dt] ,[Agv_id] ,[Type] ,[Reset]) VALUES ('" +
                                    ds_aGVWarn.Tables[0].Rows[i][0] + "'," + ds_aGVWarn.Tables[0].Rows[i][3] + ",'" + ds_aGVWarn.Tables[0].Rows[i][5] + "','" + DateTime.Now.ToString() + "')");
                            }
                            ds_aGVWarn.Tables[0].Rows.RemoveAt(i);
                            i--;
                        }
                    }
                    else
                    {
                        if (Convert.ToInt16(ds_aGVWarn.Tables[0].Rows[i][3]) == del_agv_no &&
                            ds_aGVWarn.Tables[0].Rows[i][5].ToString() == AlarmInfo)
                        {
                            if (!ds_aGVWarn.Tables[0].Rows[i][5].ToString().Contains("障碍物"))
                            {
                                Command("INSERT INTO [Warn_Log] ([Dt] ,[Agv_id] ,[Type] ,[Reset]) VALUES ('" +
                                    ds_aGVWarn.Tables[0].Rows[i][0] + "'," + ds_aGVWarn.Tables[0].Rows[i][3] + ",'" + ds_aGVWarn.Tables[0].Rows[i][5] + "','" + DateTime.Now.ToString() + "')");
                            }
                            ds_aGVWarn.Tables[0].Rows.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Assembly_Name"></param>
        /// <param name="Assembly_Id"></param>
        /// <param name="AGV_Internal_No"></param>
        /// <param name="LandMark"></param>
        /// <param name="Info"></param>
        /// <param name="agv_no"></param>
        public static void CrossInfo_Insert(string Assembly_Name, string Assembly_Id, int AGV_Internal_No, int LandMark, string Info, int agv_no, int plc_value)
        {
            lock (obj_crossinfo_insert)
            {
                crossInfo_Insert(Assembly_Name, Assembly_Id, AGV_Internal_No, LandMark, Info, agv_no, plc_value);
            }
        }
        private static object obj_crossinfo_insert = new object();
        public static void crossInfo_Insert(string Assembly_Name, string Assembly_Id, int AGV_Internal_No, int LandMark, string Info, int agv_no, int plc_value)
        {
            string strsql = "";
            strsql = "INSERT INTO [Go_Log] ([Dt] ,[Agv_id] ,[Type] ,[Location],[plc_value])  VALUES " +
                "('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'," + agv_no + ",'" + Info + "'," + LandMark + "," + plc_value + ")";
            //DataRow dr = ds_AGVGo.Tables[0].NewRow();
            //dr[0] = DateTime.Now.ToString();   //时间 时间
            //dr[1] = Assembly_Id;   //线体编号 线体编号
            //dr[2] = Assembly_Name;      //线体名称 线体名称
            //dr[3] = agv_no;      //设备编号 设备编号
            //dr[4] = AGV_Internal_No;      //内部编号 内部编号
            //dr[5] = LandMark;     //位置
            //dr[6] = Info;  //放行信息
            //ds_AGVGo.Tables[0].Rows.Add(dr); //放行日志
            Command(strsql);
        }




        //本函数为AGV的失联计数器。负几表示失联几次
        private static object SEM_LOST_CONNECT = new object();
        private static void LostConnect(ref int iStatus)    //AGV的状态
        {
            lock (SEM_LOST_CONNECT)
            {
                if (iStatus >= 0)
                    iStatus = -1;
                else
                    iStatus--;
            }
        }

        private static void WarnInfo(int warn_id, out string info, out int level)
        {
            info = "";
            level = 0;
            foreach (DataRow dr in ds_warn.Tables[0].Rows)
            {
                if (Convert.ToInt16(dr[0]) == warn_id)
                {
                    info = dr[1].ToString();
                    level = Convert.ToInt16(dr[2]);
                    return;
                }
            }
        }

        private static object obj_GetLandMarkId = new object();
        /// <summary>
        /// 根据地标值读取地标编号
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetLandMarkId(int assemblyline_id, int value)
        {
            lock (obj_GetLandMarkId)
            {
                int markid = 0;
                DataView rowfilter = new DataView(ds_location.Tables[0]);
                rowfilter.RowFilter = "Flag_Value=" + value + " and Assembly_ID= " + assemblyline_id;
                rowfilter.RowStateFilter = DataViewRowState.OriginalRows;
                DataTable dt = rowfilter.ToTable();
                try
                {
                    markid = Convert.ToInt16(dt.Rows[0]["Landmark_ID"].ToString());
                }
                catch
                {
                }
                return markid;
            }
        }


        private static object obj_GetMileage = new object();
        public static float GetMileage(int point1, int point2)
        {
            lock (obj_GetMileage)
            {
                float mileage = 0;
                int t = ds_mileage.Tables[0].Rows.Count;
                DataView rowfilter = new DataView(ds_mileage.Tables[0]);
                rowfilter.RowFilter = "(Start_Point=" + point1 + " or Start_Point=" + point2 + ") and (End_Point=  " + point1 + " or End_Point=  " + point2 + ")";
                rowfilter.RowStateFilter = DataViewRowState.OriginalRows;
                DataTable dt = rowfilter.ToTable();
                int t1 = dt.Rows.Count;
                try
                {
                    mileage = Convert.ToSingle(dt.Rows[0]["Distance"].ToString());
                }
                catch
                {
                }
                return mileage;
            }
        }
    }
}
