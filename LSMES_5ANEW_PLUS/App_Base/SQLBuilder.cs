using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using LSMES_5ANEW_PLUS.App_Base;

namespace LSMES_5ANEW_PLUS
{
    public class SQLQueryBuilder
    {
        private string mBomno;
        private string mPipeline;
        private string mDatatable;
        private string mTablehead;
        private string mShortbom;
        private bool mDistinct;
        private StringBuilder mSQLString;
        private StringBuilder mConditionString;
        private StringBuilder mOrderByString;
        private StringBuilder mFields, mValues, mOrderBy;
        private QueryType mQT;
        public string Bomno
        {
            set { mBomno = value; }
            get { return mBomno; }
        }
        public string Pipeline
        {
            set { mPipeline = value; }
            get { return mPipeline; }
        }
        public string DataTable
        {
            set { mDatatable = value; }
            get { return mDatatable; }
        }
        public string TableHead
        {
            set { mTablehead = value; }
            get { return mTablehead; }
        }
        public string ShortBom
        {
            set { mShortbom = value; }
            get { return mShortbom; }
        }
        public bool Distinct
        {
            set { mDistinct = value; }
            get { return mDistinct; }
        }
        public SQLQueryBuilder(QueryType T)
        {
            mQT = T;
            mSQLString = new StringBuilder();
            mConditionString = new StringBuilder();
            mOrderByString = new StringBuilder();
            mFields = new StringBuilder();
            mValues = new StringBuilder();
            mOrderBy = new StringBuilder();
        }
        /// <summary>
        /// 组装SQLQuery
        /// </summary>
        /// <param name="field">字段名称</param>
        /// <param name="explain">select:别名；insert:字段值；update:字段值</param>
        /// <param name="value">在QueryType.INSERT下，是对应插入字段的值</param>
        public void Assemble(string field, string explain,string value)
        {
            try
            {
                if (mQT == QueryType.SELECT)
                {
                    mSQLString.Append(field);
                    mSQLString.Append(" as [");
                    mSQLString.Append(explain);
                    mSQLString.Append("],");
                }
                else if (mQT == QueryType.INSERT)
                {
                    mFields.Append(field);
                    mFields.Append(",");
                    mValues.Append("'");
                    mValues.Append(value);
                    mValues.Append("',");
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
            }
        }
        /// <summary>
        /// 组装条件
        /// </summary>
        /// <param name="field">字段名称</param>
        /// <param name="value">字段值</param>
        /// <param name="state">true：与关系；false：或关系</param>
        public void Condition(string field, string value, bool state)
        {
            try
            {
                if (value.ToUpper() == "IS NOT NULL")
                {
                    mConditionString.Append(field);
                    mConditionString.Append(" IS NOT NULL");
                    if (state)
                    {
                        mConditionString.Append(" and ");
                    }
                    else
                    {
                        mConditionString.Append(" or  ");
                    }
                }
                else if (value != "")
                {
                    mConditionString.Append(field);
                    mConditionString.Append("='");
                    mConditionString.Append(value);
                    if (state)
                    {
                        mConditionString.Append("' and ");
                    }
                    else
                    {
                        mConditionString.Append("' or  ");
                    }
                }
                else
                {
                    mConditionString.Append(field);
                    mConditionString.Append(" is NULL");
                    if (state)
                    {
                        mConditionString.Append(" and ");
                    }
                    else
                    {
                        mConditionString.Append(" or  ");
                    }
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
            }
        }
        /// <summary>
        /// Order by 条件
        /// </summary>
        /// <param name="field">字段名称</param>
        /// <param name="order">ASC：升序；DESC：降序</param>
        public void OrderBy(string field,string order)
        {
            try
            {
                if (mQT == QueryType.SELECT)
                {
                    mOrderByString.Append(" order by ");
                    mOrderByString.Append(field);
                    mOrderByString.Append(" ");
                    mOrderByString.Append(order);
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
            }
        }
        /// <summary>
        /// 创建SQLQuery
        /// </summary>
        /// <returns>SQLQuery</returns>
        public string Build()
        {
            string query;
            if (mQT == QueryType.SELECT)
            {
                if (mDistinct)
                {
                    mSQLString.Insert(0, "select distinct ");
                }
                else
                {
                    mSQLString.Insert(0, "select ");
                }
                mSQLString = mSQLString.Remove(mSQLString.Length - 1, 1);
                mSQLString.Append(" from ");
                mSQLString.Append(mDatatable);
                if (mConditionString.Length!=0)
                {
                    mConditionString.Insert(0, " where ");
                    mConditionString = mConditionString.Remove(mConditionString.Length - 5, 5);
                }
                mSQLString.Append(mConditionString);
                if (mOrderByString.Length != 0)
                {
                    mSQLString.Append(mOrderByString);
                }
            }
            else if (mQT == QueryType.INSERT)
            {
                mSQLString.Clear();
                mSQLString.Append("insert into ");
                mSQLString.Append(mDatatable);
                mSQLString.Append("(");
                mSQLString.Append(mFields.Remove(mFields.Length-1,1));
                mSQLString.Append(") values (");
                mSQLString.Append(mValues.Remove(mValues.Length-1,1));
                mSQLString.Append(")");
                mSQLString.Replace("''", "NULL");
            }
            else if (mQT == QueryType.UPDATE)
            {
                //mSQLString.Insert(0,"update "+mDatatable);
            }
            else if (mQT == QueryType.DELETE)
            {
                //mSQLString.Insert(0,"delete "+mDatatable);
            }
            query = mSQLString.ToString();
            mSQLString.Clear();
            mConditionString.Clear();
            return query;
        }
    }
}