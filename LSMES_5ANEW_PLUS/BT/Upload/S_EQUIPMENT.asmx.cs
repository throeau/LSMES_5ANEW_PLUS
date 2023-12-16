﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;
using LSMES_5ANEW_PLUS.Business;


namespace LSMES_5ANEW_PLUS.BT.Upload
{
    /// <summary>
    /// S_EQUIPMENT 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class S_EQUIPMENT : System.Web.Services.WebService
    {

        [WebMethod]
        public XmlDocument updateSE(string equipmentno, string state, string recieved_date_time)
        {
            try
            {
                //初始化一个xml实例
                XmlDocument myXmlDoc = new XmlDocument();
                //创建xml的根节点
                XmlElement rootElement = myXmlDoc.CreateElement("ROOT");
                //将根节点加入到xml文件中（AppendChild）
                myXmlDoc.AppendChild(rootElement);
                //初始化第一层的第一个子节点
                XmlElement firstLevelElement1 = myXmlDoc.CreateElement("RESULT");
                //填充第一层的第一个子节点的属性值（SetAttribute）
                firstLevelElement1.SetAttribute("DATE", DateTime.Now.ToString());
                State mState = new State();
                firstLevelElement1.InnerText = mState.UpdateStateByEquipment(equipmentno,state,recieved_date_time).ToString();
                //将第一层的第一个子节点加入到根节点下
                rootElement.AppendChild(firstLevelElement1);
                return myXmlDoc;
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
    }
}
