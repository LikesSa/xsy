using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Web.Script.Serialization;
using xsy.likes.Base;
using xsy.likes.WebServices;

namespace xsy.likes.pwmd
{
    public class pwapi
    {
        private readonly string userId;
        private readonly string pwd;
        private readonly string code;
        private readonly string tokenClientId;
        private readonly string tokenClientSecret;
        private readonly string tokenRedirectUrl;
        private string apiurl = ConfigAppSetting.AppSettingsGet("apiurl");


        internal protected GetTokenReturn TokenReturn;
        protected readonly JavaScriptSerializer Json = new JavaScriptSerializer();

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pwd"></param>
        /// <param name="code"></param>
        /// <param name="tokenClientId"></param>
        /// <param name="tokenClientSecret"></param>
        /// <param name="tokenRedirectUrl"></param>
        public pwapi(string userId, string pwd, string code, string tokenClientId, string tokenClientSecret, string tokenRedirectUrl)
        {
            this.userId = userId;
            this.pwd = pwd;
            this.code = code;
            this.tokenClientId = tokenClientId;
            this.tokenClientSecret = tokenClientSecret;
            this.tokenRedirectUrl = tokenRedirectUrl;
        }


        /// <summary>
        /// 默认属性astoken
        /// </summary>
        internal protected string AccessToken
        {
            get
            {
                //GetToken();
                if (TokenReturn == null)
                {
                    TokenReturn = new GetTokenReturn();
                }
                if (string.IsNullOrEmpty(TokenReturn.issued_at))
                {
                    GetToken();
                }
                var minute =
                   (DateTime.Now - DateTime.Parse("1970-1-1").AddMilliseconds(double.Parse(TokenReturn?.issued_at)))
                       .TotalMinutes;
                if (minute >= 24 * 60)
                {
                    GetToken();
                }
                return TokenReturn?.access_token;
            }
        }

        /// <summary>
        /// 获取astoken
        /// </summary>
        protected virtual void GetToken()
        {
            var url =
                $"{apiurl}/oauth2/token?grant_type=password&client_id={tokenClientId}&client_secret={tokenClientSecret}&redirect_uri={tokenRedirectUrl}&username={userId}&password={pwd}{code}";

            WebHelper.Instrance.RequestGetDataShaderJson(url, out TokenReturn,"utf-8");
        }


        /// <summary>
        /// 接口测试
        /// </summary>
        /// <returns></returns>
        public string inTextBygetoke()
        {
            return AccessToken;

        }

        #region   审批
        //获取所有待审批
        public virtual string approvalsGet(string pageNo,string pageSize,string userId)
        {
            var url =
                $"{apiurl}/data/v1/objects/approval/approvals?pageNo={pageNo}&pageSize={pageSize}&userId={userId}&access_token={AccessToken}";

            string result;
            WebHelper.Instrance.RequestGetData(url, out result, "utf-8");
            return result;
        }

        //审批流程定义
        public virtual string approvalsLcGet(string belongId,string entityType)
        {
            //https://api.xiaoshouyi.com/data/v1/objects/approval/define
            var url =
                $"{apiurl}/data/v1/objects/approval/define?belongId={belongId}&entityType={entityType}&access_token={AccessToken}";

            string result;
            WebHelper.Instrance.RequestGetData(url, out result, "utf-8");
            return result;
        }

        //获取下一级审批提交人
        public virtual string approvalsNextGet(string belongId,string dataId, string approvalId,string defineId)
        {
            //https://api.xiaoshouyi.com/data/v1/objects/approval/define
            string  url = string.Empty;
            if (string.IsNullOrEmpty(approvalId))
            {
                 url =
                        $"{apiurl}/data/v1/objects/approval/nextUser?belongId={belongId}&dataId={dataId}&defineId={defineId}&access_token={AccessToken}";

            }
            else
            {
                 url =
                    $"{apiurl}/data/v1/objects/approval/nextUser?belongId={belongId}&dataId={dataId}&approvalId={approvalId}&defineId={defineId}&access_token={AccessToken}";

            }

            string result;
            WebHelper.Instrance.RequestGetData(url, out result, "utf-8");
            return result;
        }




        //提交审批
        public virtual bool approvalsLcPut(string json, string charset = "utf-8")
        {
            string url = $"{apiurl}/data/v1/objects/approval/submit";
            var input = $"access_token={AccessToken}&json={json}&charset={charset}";
            string result;
            var webResult = WebHelper.Instrance.RequestPostData(url,input, out result, charset);

            var status = string.IsNullOrWhiteSpace(result) ? "" : result.Substring(result.IndexOf(":", StringComparison.Ordinal) + 1, result.IndexOf("}", StringComparison.Ordinal) - result.IndexOf(":", StringComparison.Ordinal) - 1);

            if (status.Contains("\"1002002\",") || result.Contains("error_code\":\"300001")) ////|| result.Contains("error_code\":\"300001")
            {
                status = "0";
            }
            return status.Trim().Equals("0");
        }


        //通过审批
        public virtual bool approvalsLcPass(string json, string charset = "utf-8")
        {
            string url = $"{apiurl}/data/v1/objects/approval/agree";
            var input = $"access_token={AccessToken}&json={json}&charset={charset}";
            string result;
            var webResult = WebHelper.Instrance.RequestPostData(url,input, out result, charset);

            var status = string.IsNullOrWhiteSpace(result) ? "" : result.Substring(result.IndexOf(":", StringComparison.Ordinal) + 1, result.IndexOf("}", StringComparison.Ordinal) - result.IndexOf(":", StringComparison.Ordinal) - 1);

            if (status.Contains("\"1002002\",") || result.Contains("error_code\":\"300001")) ////|| result.Contains("error_code\":\"300001")
            {
                status = "0";
            }
            return status.Trim().Equals("0");
        }

        #endregion
        #region  通用模块

        /// <summary>
        /// SQL查询通用方法获取返回的完整json
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public virtual string sqlSelect(string sql, string charset = "utf-8")
        {
            string url = $"{apiurl}/data/v1/query";


            string result;
            string code = $"Bearer {AccessToken}";
            string input = $"q={sql}";
            WebHelper.Instrance.RequestPostAddHeadData(url, input, code, out result, charset);
            return result;
            //string result;
            //result = WebHelper.Instrance.HttpPost(url);

            //return result;
        }

        /// <summary>
        /// SQL查询通用方法获取id
        /// </summary>
        /// <param name="sql"></param>
        public virtual string sqlSelectOutId(string sql, string charset = "utf-8")
        {
            string url = $"{apiurl}/data/v1/query?access_token={AccessToken}&q={sql}";

            string result;

            result = WebHelper.Instrance.HttpPost(url);
            if (!result.Contains("totalSize\":0") && !string.IsNullOrWhiteSpace(result) && result.Contains("id"))
            {
                return string.IsNullOrWhiteSpace(result) ? "" : result.Substring(result.IndexOf("id\":", StringComparison.Ordinal) + 4, result.IndexOf("}", StringComparison.Ordinal) - result.IndexOf("id\":", StringComparison.Ordinal) - 4);
            }
            else
            {
                return null;
            }
        }




        /// <summary>
        /// 添加的通用接口
        /// </summary>
        /// <param name="url"></param>
        /// <param name="json"></param>
        /// <param name="id"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        private bool InterfaceCreate(string url, string json, out string id)
        {
            var input = $"access_token={AccessToken}&json={json}";

            string result;
            var webResult = WebHelper.Instrance.RequestPostData(url, input, out result); //"application/json"

            if (!webResult || string.IsNullOrWhiteSpace(result) || result.ToUpper().Contains("ERROR"))
            {
                if (result.Contains("数据重复"))
                {
                    id = "数据重复";
                }
                else if (result.Contains("duplicate record"))
                {
                    id = "数据重复";

                }
                else if (result.Contains("1002002"))
                {
                    id = "数据重复";
                }

                else
                {
                    id = result;
                }
                return false;
            }

            id = string.IsNullOrWhiteSpace(result)
                ? ""
                : result.Substring(result.IndexOf(":", StringComparison.Ordinal) + 1,
                    result.IndexOf("}", StringComparison.Ordinal) - result.IndexOf(":", StringComparison.Ordinal) -
                    1);


            return true;
        }


        /// <summary>
        /// 修改的公共接口
        /// </summary>
        /// <param name="url"></param>
        /// <param name="json"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        private bool InterfaceUpdate(string url, string json, string charset = "utf-8")
        {
            string result;
            string code = $"Bearer {AccessToken}";
            WebHelper.Instrance.RequestPostAddHeadData(url, json, code, out result, charset, "application/son");

            var status = string.IsNullOrWhiteSpace(result) ? "" : result.Substring(result.IndexOf(":", StringComparison.Ordinal) + 1, result.IndexOf("}", StringComparison.Ordinal) - result.IndexOf(":", StringComparison.Ordinal) - 1);

            if (status.Contains("\"1002002\",") || result.Contains("error_code\":\"300001")) ////|| result.Contains("error_code\":\"300001")
            {
                status = "0";
            }
            return status.Trim().Equals("0");
        }
        #endregion

        #region 客户
        /// <summary>
        /// 获取客户描述信息
        /// </summary>
        public virtual string CustomerInterfaceSelect()
        {
            var url =
                $"{apiurl}/data/v1/objects/account/describe?access_token={AccessToken}";

            string result;
            WebHelper.Instrance.RequestGetData(url, out result, "utf-8");
            return result;
        }

        /// <summary>
        /// 创建
        /// </summary>
        protected virtual string CustomerCreate(string json, string charset = "utf-8")
        {
            var url = $"{apiurl}/data/v1/objects/account/create";  //https://testapi.xiaoshouyi.com

            var input = $"access_token={AccessToken}&json={Uri.EscapeDataString(json)}&charset={charset}";
            string result;
            WebHelper.Instrance.RequestPostData(url, input, out result, charset);

            return string.IsNullOrWhiteSpace(result) ? "" : result.Substring(result.IndexOf(":", StringComparison.Ordinal) + 1, result.IndexOf("}", StringComparison.Ordinal) - result.IndexOf(":", StringComparison.Ordinal) - 1);
        }

        /// <summary>
        /// 删除
        /// </summary>
        public virtual string CustomerDelete(string id)
        {
            string url = $"{apiurl}/data/v1/objects/account/delete";

            string result;
            var input = $"access_token={AccessToken}&id={id}";
            WebHelper.Instrance.RequestPostData(url, input, out result, "utf-8");

            return string.IsNullOrWhiteSpace(result) ? "" : result.Substring(result.IndexOf(":", StringComparison.Ordinal) + 1, result.IndexOf("}", StringComparison.Ordinal) - result.IndexOf(":", StringComparison.Ordinal) - 1);
        }

        /// <summary>
        /// 更改
        /// </summary>
        protected virtual string CustomerUpdate(string json, string charset = "utf-8")
        {
            string url = $"{apiurl}/data/v1/objects/account/update";

            string result;
            string code = $"Bearer {AccessToken}";
            WebHelper.Instrance.RequestPostAddHeadData(url, json,code, out result, charset,"application/son");

            return string.IsNullOrWhiteSpace(result) ? "" : result.Substring(result.IndexOf(":", StringComparison.Ordinal) + 1, result.IndexOf("}", StringComparison.Ordinal) - result.IndexOf(":", StringComparison.Ordinal) - 1);
        }

        /// <summary>
        /// 根据客户id获取客户明细
        /// </summary>
        protected virtual string CustomerSelect(string id)
        {
            string url = $"{apiurl}/data/v1/objects/account/info?access_token={AccessToken}&id={id}";

            string result;
            WebHelper.Instrance.RequestGetData(url, out result,"utf-8");

            return result;
        }

        /// <summary>
        /// 转移给别的用户
        /// </summary>
        public virtual string CustomerTransfer(string id, string targetOwnerId)
        {
            string url = $"https://api.xiaoshouyi.com/data/v1/objects/account/transfer";

            string result;
            var input = $"access_token={AccessToken}&id={id}&targetOwnerId={targetOwnerId}";
            WebHelper.Instrance.RequestPostData(url, input, out result, "BGK");

            return string.IsNullOrWhiteSpace(result) ? "" : result.Substring(result.IndexOf(":", StringComparison.Ordinal) + 1, result.IndexOf("}", StringComparison.Ordinal) - result.IndexOf(":", StringComparison.Ordinal) - 1);
        }

        /// <summary>
        /// 退回公海池
        /// </summary>
        public virtual string CutomerRelease(string releaseReason, string accountId, string reasonType)
        {
            string url = $"https://api.xiaoshouyi.com/data/v1/objects/account/release";

            string result;
            var input = $"access_token={AccessToken}&accountId={accountId}&reasonType={reasonType}&releaseReason={releaseReason}";
            WebHelper.Instrance.RequestPostData(url,input, out result, "utf-8");

            return string.IsNullOrWhiteSpace(result) ? "" : result.Substring(result.IndexOf(":", StringComparison.Ordinal) + 1, result.IndexOf("}", StringComparison.Ordinal) - result.IndexOf(":", StringComparison.Ordinal) - 1);
        }
        #endregion

        #region 用户
        /// <summary>
        /// 获取用户描述信息
        /// </summary>
        public virtual string UserInterfaceSelect()
        {
            var url =
                $"{apiurl}/data/v1/objects/user/describe?access_token={AccessToken}";

            string result;
            WebHelper.Instrance.RequestGetData(url, out result, "utf-8");
            return result;
        }

        public virtual string UserGetList()
        {
            var url =
                $"{apiurl}/data/v1/objects/user/list?access_token={AccessToken}";

            string result;
            WebHelper.Instrance.RequestGetData(url, out result, "utf-8");
            return result;
        }
        /// <summary>
        /// 根据客户id获取用户明细
        /// </summary>
        protected virtual string UserSelectById(string id)
        {
            string url = $"{apiurl}/data/v1/objects/user/info?access_token={AccessToken}&id={id}";

            string result;
            WebHelper.Instrance.RequestGetData(url, out result,"utf-8");

            return result;
        }

        /// <summary>
        /// 根据用户姓名获取用户明细
        /// </summary>
        protected virtual string UserSelectByName(string name)
        {
            //https://api.xiaoshouyi.com/data/v1/objects/user/detail?
            string url = $"{apiurl}/data/v1/objects/user/detail?access_token={AccessToken}&name={name}";

            //string result;
            // result = WebHelper.Instrance.HttpPost(url);
            //WebHelper.Instrance.RequestGetData(url, out result);

            return WebHelper.Instrance.HttpPost(url); ;
        }


        #endregion

        #region 自定义
        /// <summary>
        /// 获取描述
        /// </summary>
        public virtual string CustomInterfaceSelect(string belongId)
        {
            //https://api.xiaoshouyi.com/data/v1/objects/customize/describe
            var url =
                $"{apiurl}/data/v1/objects/customize/describe?belongId={belongId}&access_token={AccessToken}";

            string result;
            WebHelper.Instrance.RequestGetData(url, out result, "utf-8");
            return result;
        }

        /// <summary>
        /// 创建
        /// </summary>
        public virtual bool CustomCreate(string json, out string id)
        {
            string url = $"{apiurl}/data/v1/objects/customize/create";
            return InterfaceCreate(url, json, out id); //Uri.EscapeDataString(json)
        }

        /// <summary>
        /// 更改
        /// </summary> 
        public virtual bool CustomUpdate(string json, string charset = "utf-8")
        {
            string url = $"{apiurl}/data/v1/objects/customize/update";
            return InterfaceUpdate(url, json, charset); //Uri.EscapeDataString(json)
        }

        /// <summary>
        /// 删除
        /// </summary>
        public virtual bool CustomizeDelete(string id, out string result, string charset = "utf-8")
        {
            string url = $"{apiurl}/data/v1/objects/customize/delete?id={id}&access_token={AccessToken}";
            var input = $"charset={charset}";
            WebHelper.Instrance.RequestPostData(url, input, out result, charset);

            return true;

        }

        /// <summary>
        /// 根据id获取明细
        /// </summary>
        protected virtual string CustomSelect(string id,string charset = "utf-8")
        {
            //https://api.xiaoshouyi.com/data/v1/objects/customize/info
            string url = $"{apiurl}/data/v1/objects/customize/info?access_token={AccessToken}&id={id}";

            string result;
            WebHelper.Instrance.RequestGetData(url, out result,charset);

            return result;
        }

        #endregion

        #region 上传文件
        protected virtual string fileGetList()
        {
            var url =
                $"{apiurl}/data/v1/data/v1/objects/document/list?access_token={AccessToken}";
 
            string result;
            WebHelper.Instrance.RequestGetData(url, out result, "utf-8");
            return result;
        }

        #endregion

        #region 销售机会

        /// <summary>
        /// 获取销售机会描述
        /// </summary>
        public virtual string OpportunityGetInFo()
        {
            var url =
                $"{apiurl}/data/v1/objects/opportunity/describe?access_token={AccessToken}";

            string result;
            WebHelper.Instrance.RequestGetData(url, out result, "utf-8");
            return result;
        }

        /// <summary>
        /// 根据销售机会id获取销售机会明细
        /// </summary>
        protected virtual string OpInfoById(string id)
        {
            string url = $"{apiurl}/data/v1/objects/opportunity/info?access_token={AccessToken}&id={id}";

            string result;
            WebHelper.Instrance.RequestGetData(url, out result,"utf-8");

            return result;
        }

        //更改销售机会
        protected virtual string OpportunityUpdate(string json, string charset = "utf-8")
        {
            string url = $"{apiurl}/data/v1/objects/opportunity/update";

            string result;
            var input = $"access_token={AccessToken}&json={Uri.EscapeDataString(json)}&charset={charset}";
            WebHelper.Instrance.RequestPostData(url, input, out result, charset);

            return string.IsNullOrWhiteSpace(result) ? "" : result.Substring(result.IndexOf(":", StringComparison.Ordinal) + 1, result.IndexOf("}", StringComparison.Ordinal) - result.IndexOf(":", StringComparison.Ordinal) - 1);
        }


        #endregion


        #region  产品
        /// <summary>
        /// 获取描述信息
        /// </summary>
        public virtual string ProductInterfaceSelect()
        {
            var url =
                $"{apiurl}/data/v1/objects/product/describe?access_token={AccessToken}";

            string result;
            WebHelper.Instrance.RequestGetData(url, out result, "utf-8");
            return result;
        }

        /// <summary>
        /// 创建
        /// </summary>
        protected virtual string ProductCreate(string json, string charset = "utf-8")
        {
            var url = $"{apiurl}/data/v1/objects/product/create";  //https://testapi.xiaoshouyi.com

            var input = $"access_token={AccessToken}&json={Uri.EscapeDataString(json)}&charset={charset}";
            string result;
            WebHelper.Instrance.RequestPostData(url, input, out result, charset);

            return string.IsNullOrWhiteSpace(result) ? "" : result.Substring(result.IndexOf(":", StringComparison.Ordinal) + 1, result.IndexOf("}", StringComparison.Ordinal) - result.IndexOf(":", StringComparison.Ordinal) - 1);
        }

        /// <summary>
        /// 删除
        /// </summary>
        public virtual string ProductDelete(string id)
        {
            string url = $"{apiurl}/data/v1/objects/account/delete";

            string result;
            var input = $"access_token={AccessToken}&id={id}";
            WebHelper.Instrance.RequestPostData(url,input, out result, "utf-8");

            return string.IsNullOrWhiteSpace(result) ? "" : result.Substring(result.IndexOf(":", StringComparison.Ordinal) + 1, result.IndexOf("}", StringComparison.Ordinal) - result.IndexOf(":", StringComparison.Ordinal) - 1);
        }

        /// <summary>
        /// 更改
        /// </summary>
        protected virtual string ProductUpdate(string json, string charset = "utf-8")
        {
            string url = $"{apiurl}/data/v1/objects/product/update";

            string result;
            var input = $"access_token={AccessToken}&json={Uri.EscapeDataString(json)}&charset={charset}";
            WebHelper.Instrance.RequestPostData(url, input, out result, charset);

            return string.IsNullOrWhiteSpace(result) ? "" : result.Substring(result.IndexOf(":", StringComparison.Ordinal) + 1, result.IndexOf("}", StringComparison.Ordinal) - result.IndexOf(":", StringComparison.Ordinal) - 1);
        }

        /// <summary>
        /// 根据客户id获取客户明细
        /// </summary>
        protected virtual string ProductrSelect(string id)
        {
            string url = $"{apiurl}/data/v1/objects/product/info?access_token={AccessToken}&id={id}";

            string result;
            WebHelper.Instrance.RequestGetData(url, out result,"utf-8");

            return result;
        }

        //获取产品目录
        public virtual string ProductrTreeGet(String charset = "utf-8")
        {

            string url = $"{apiurl}/data/v1/objects/product/directoryList?parentId={0}&page={1}&count={30}";

            string result;
            string code = $"Bearer {AccessToken}";
            WebHelper.Instrance.RequestGettAddHeadData(url, code, out result, charset);

            return result;


        }

        //获取产品列表
        public virtual string ProductrListGet(String ProductrTreeid,String charset = "utf-8")
        {

            string url = $"{apiurl}/data/v1/objects/product/list?parentId={ProductrTreeid}&page={1}&count={30}";

            string result;
            string code = $"Bearer {AccessToken}";
            WebHelper.Instrance.RequestGettAddHeadData(url, code, out result, charset);

            return result;


        }

        #endregion

        #region 发送通知

        protected virtual string MessageSend(string json, string charset = "utf-8")
        {
            string url = $"{apiurl}/data/v1/notice/notify/send";

            string result;
            string code = $"Bearer {AccessToken}";
            bool fa = WebHelper.Instrance.RequestPostAddHeadData(url, json, code, out result, charset, "application/son");

            return result;

        }
        #endregion

        #region 部门
        /// <summary>
        /// 查看部门树
        /// </summary>
        public virtual string DepartTreeGet()
        {
            //https://api.xiaoshouyi.com/data/v1/objects/depart/tree
            var url =
                $"{apiurl}/data/v1/objects/depart/tree?access_token={AccessToken}";

            string result;
            WebHelper.Instrance.RequestGetData(url, out result, "utf-8");
            return result;
        }
        /// <summary>
        /// 根据id获取部门明细
        /// </summary>
        protected virtual string DepartMxGet(string id)
        {

            //https://api.xiaoshouyi.com/data/v1/objects/depart/info?id=25314
            string url = $"{apiurl}/data/v1/objects/depart/info?access_token={AccessToken}&id={id}";

            string result;
            WebHelper.Instrance.RequestGetData(url, out result,"utf-8");

            return result;
        }

        /// <summary>
        /// 根据id获取部门名称
        /// </summary>
        protected virtual string DepartNameGet(string id)
        {

            //https://api.xiaoshouyi.com/data/v1/objects/depart/info?id=25314
            string url = $"{apiurl}/data/v1/objects/depart/info?access_token={AccessToken}&id={id}";

            string result;
            WebHelper.Instrance.RequestGetData(url, out result,"utf-8");

            if (result.Contains("departName"))
            {
                return result.Substring(result.IndexOf("departName") + 13,(result.IndexOf("parentDepartId") - result.IndexOf("departName") -16));

            }
            else

            return null;
        }


        #endregion


        #region
        public virtual string OrderInterfaceSelect()
        {
            var url =
                $"{apiurl}/data/v1/objects/order/describe?access_token={AccessToken}";  //https://api.xiaoshouyi.com/data/v1/objects/order/describe

            string result;
            WebHelper.Instrance.RequestGetData(url, out result, "utf-8");
            return result;
        }

        /// <summary>
        /// 创建
        /// </summary>
        protected virtual string OrderCreate(string json, string charset = "utf-8")
        {
            var url = $"{apiurl}/data/v1/objects/order/create";  //https://testapi.xiaoshouyi.com

            var input = $"access_token={AccessToken}&json={Uri.EscapeDataString(json)}&charset={charset}";
            string result;
            WebHelper.Instrance.RequestPostData(url,input, out result, charset);

            return string.IsNullOrWhiteSpace(result) ? "" : result.Substring(result.IndexOf(":", StringComparison.Ordinal) + 1, result.IndexOf("}", StringComparison.Ordinal) - result.IndexOf(":", StringComparison.Ordinal) - 1);
        }

        /// <summary>
        /// 删除
        /// </summary>
        public virtual string OrderDelete(string id)
        {
            string url = $"{apiurl}/data/v1/objects/order/delete";

            string result;
            var input = $"access_token={AccessToken}&id={id}";
            WebHelper.Instrance.RequestPostData(url, input, out result, "utf-8");

            return string.IsNullOrWhiteSpace(result) ? "" : result.Substring(result.IndexOf(":", StringComparison.Ordinal) + 1, result.IndexOf("}", StringComparison.Ordinal) - result.IndexOf(":", StringComparison.Ordinal) - 1);
        }

        /// <summary>
        /// 更改
        /// </summary>
        protected virtual string OrderUpdate(string json, string charset = "utf-8")
        {
            string url = $"{apiurl}/data/v1/objects/order/update";

            string result;
            var input = $"access_token={AccessToken}&json={Uri.EscapeDataString(json)}&charset={charset}";
            WebHelper.Instrance.RequestPostData(url, input, out result, charset);

            return string.IsNullOrWhiteSpace(result) ? "" : result.Substring(result.IndexOf(":", StringComparison.Ordinal) + 1, result.IndexOf("}", StringComparison.Ordinal) - result.IndexOf(":", StringComparison.Ordinal) - 1);
        }

        #endregion


        //工作报告
        public virtual string workReportGet()
        {
            var url =
                $"{apiurl}/data/v1/objects/workReport/list?access_token={AccessToken}";

            string result;
            WebHelper.Instrance.RequestGetData(url, out result, "utf-8");
            return result;
        }

        #region xoql
        public virtual string xoqlGet(string sql, string charset = "utf-8")
        {
           
            string url = $"{apiurl}/rest/data/v2.0/query/xoql?";
            string input = $"xoql={sql}";

            string result;
            string code = $"Bearer {AccessToken}";
            bool fa = WebHelper.Instrance.RequestPostAddHeadData(url,input,code,out result,charset);

            return result;


        }


        public virtual string[] IdGetSql(string vl, string sql, string charset = "utf-8")
        {
            string url = string.Format("{0}/rest/data/v2.0/query/xoql?", this.apiurl);
            string input = string.Format("xoql={0}", sql);
            string rejson = string.Empty;
            string code = $"Bearer {AccessToken}";
            bool fa = WebHelper.Instrance.RequestPostAddHeadData(url, input, code, out rejson, charset, "application/x-www-form-urlencoded");

            string[] str = null;
            if (fa && rejson.Contains("\"code\":\"200\""))
            {
                JObject jObject = (JObject)JsonConvert.DeserializeObject(rejson);
                int num = Convert.ToInt32(jObject["data"]["count"].ToString());
                string[] array = new string[num];
                bool flag3 = num > 0;
                if (flag3)
                {
                    for (int i = 0; i < num; i++)
                    {
                        array[i] = jObject["data"]["records"][i][vl].ToString();
                    }
                    str = array;
                    return str;
                }
            }
            return str;
        }
        #endregion

        #region  异步
        //获取指定租户下所有异步任务详情
        protected virtual string AllDetailsGet(string id,string charset = "utf-8")
        {
            string url = $"{apiurl}/rest/bulk/v2/batch?jobId={id}";
            string result;
            string code = $"Bearer {AccessToken}";
            WebHelper.Instrance.RequestGettAddHeadData(url, code, out result, charset);

            return result;
        }

        //获取异步批量任务详情
        protected virtual string DetailsOfthebatch(string batchId, string charset = "utf-8")
        {
            string url = $"{apiurl}/rest/bulk/v2/batch/{batchId}";
            string result;
            string code = $"Bearer {AccessToken}";
            WebHelper.Instrance.RequestGettAddHeadData(url, code, out result, charset);

            return result;
        }

        //获取异步批量任务结果
        protected virtual string ResultOfthebatch(string batchId, string charset = "utf-8")
        {
            string url = $"{apiurl}/rest/bulk/v2/batch/{batchId}/result";
            string result;
            string code = $"Bearer {AccessToken}";
            WebHelper.Instrance.RequestGettAddHeadData(url, code, out result, charset);

            return result;
        }


        //获取异步作业信息
        #endregion
        protected virtual string DetailsOftheJob(string jobId, string charset = "utf-8")
        {
            string url = $"{apiurl}/rest/bulk/v2/job/{jobId}";
            string result;
            string code = $"Bearer {AccessToken}";
            WebHelper.Instrance.RequestGettAddHeadData(url, code, out result, charset);

            return result;
        }
        //获取该租户下所有异步作业详情
        protected virtual string AlljobGet(string charset = "utf-8")
        {
            string url = $"{apiurl}/rest/bulk/v2/job";
            string result;
            string code = $"Bearer {AccessToken}";
            WebHelper.Instrance.RequestGettAddHeadData(url, code, out result, charset);

            return result;
        }

        //更新异步作业信息
        protected virtual string CloseTheJob(string jobId,string charset = "utf-8")
        {
            string url = $"{apiurl}/rest/bulk/v2/job/{jobId}";
            string result;
            var input = " {\"status\": 2}";  //JsonConvert.SerializeObject(
            string code = $"Bearer {AccessToken}";
            WebHelper.Instrance.RequestPatchAddHeadData(url,input,code,out result,charset);

            return result;
        }



        #region v2接口
        //获取自定义明细
        public virtual string customizeGetById(string ApiKey,string id,string charset = "utf-8")
        {
            string url = $"{apiurl}/rest/data/v2/objects/{ApiKey}/{id}";

            string result;
            string code = $"Bearer {AccessToken}";
            WebHelper.Instrance.RequestGettAddHeadData(url,code, out result,charset);

            return result;



        }

        //获取自定义明细描述
        public virtual string customizeGet(string ApiKey,string charset = "utf-8")
        {
            string url = $"{apiurl}/rest/data/v2/objects/{ApiKey}/description";

            string result;
            string code = $"Bearer {AccessToken}";
            WebHelper.Instrance.RequestGettAddHeadData(url, code, out result, charset);

            return result;



        }







   


        //下载图片
        public virtual bool picDown(string fileId, string charset = "utf-8")
        {
            //string url = $"{apiurl}/rest/file/v2.0/image/{fileId}";

            //string result;
            //string code = $"Bearer {AccessToken}";
            //WebHelper.Instrance.RequestGettAddHeadData(url, code, out result, charset);

            //return result;
            return WebHelper.Instrance.DownloadOneFileByURL("1531447380605.jpg","https://xsybucket.s3.cn-north-1.amazonaws.com.cn/363680/2018/07/13/bdc3e2f3-7e05-4f34-be31-33077e47c204.jpg","C:\\Downlod\\dd",90);



        }

        #endregion




        public void Dispose()
        {
            TokenReturn?.Dispose();
        }

        ~pwapi()
        {
            Dispose();
        }

    }
}
