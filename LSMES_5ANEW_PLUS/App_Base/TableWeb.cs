using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace LSMES_5ANEW_PLUS.App_Base
{
    public class TableWeb
    {
        string mTable;
        string mTempTable;
        string mThead;
        int mColumnCount;
        int mTempColumnNum;
        int mRowCount;
        public void addThead(string theadCol)
        {

            mThead += "<th>";
            mThead += theadCol;
            mThead += "</th>";
            ++mColumnCount;

        }
        public void addContext(string context)
        {
            if (mColumnCount == 0)
            {
                return;
            }
            mTempTable += "<td>";
            mTempTable += context;
            mTempTable += "</td>";
            if (mTempColumnNum == mColumnCount - 1)
            {
                if (mRowCount % 2 != 0)
                {
                    mTempTable = "<tr class=\"pure-table-odd\">" + mTempTable + "</tr>";
                }
                else
                {
                    mTempTable = "<tr>" + mTempTable + "</tr>";
                }
                mTable += mTempTable;
                mTempTable = null;
                mTempColumnNum = 0;
                ++mRowCount;

            }
            else
            {
                ++mTempColumnNum;
            }
        }
        public string TableHtml()
        {
            string html = "<!DOCTYPE html><html lang=\"en\"><head><title>Table_Simple CSS for HTML tables</title><meta charset=\"UTF-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"> <style type=\"text/css\">html {    font-family: sans-serif;    -ms-text-size-adjust: 100%;    -webkit-text-size-adjust: 100%;} body {    margin: 10px;}table {    border-collapse: collapse;    border-spacing: 0;} td,th {    padding: 0;} .pure-table {    border-collapse: collapse;    border-spacing: 0;    empty-cells: show;    border: 1px solid #cbcbcb;} .pure-table caption {    color: #000;    font: italic 85%/1 arial,sans-serif;    padding: 1em 0;    text-align: center;} .pure-table td,.pure-table th {    border-left: 1px solid #cbcbcb;    border-width: 0 0 0 1px;    font-size: inherit;    margin: 0;    overflow: visible;    padding: .5em 1em;} .pure-table thead {    background-color: #e0e0e0;    color: #000;    text-align: left;    vertical-align: bottom;} .pure-table td {    background-color: transparent;} .pure-table-odd td {    background-color: #f2f2f2;}</style></head><body>    <table class=\"pure-table\">        <thead>  <tr>          {0}     <tr>   </thead>            <tbody>            {1}        </tbody>    </table></body></html>";
            try
            {
                html = html.Replace("{0}", mThead);
                html = html.Replace("{1}", mTable);
                return html;
            }
            catch(Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
    }
}