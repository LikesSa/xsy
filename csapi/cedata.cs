using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xsy.likes.pwmd;

namespace csapi
{
    public class cedata : pwapi
    {
        #region 配置文件和参数
        private const string UserId = "13782059196";     //用户名
        private const string Pwd = "123456";
        private const string Code = "RgM7PtjI";          //安全令牌
        private const string TokenClientId = "a3245dac2472896eef7607fffd5e2ed3";
        private const string TokenClientSecret = "647383ee6dac65451ab18a493ec1affb";
        private const string TokenRedirectUrl = "https://api.daoqincs.com";

        public cedata() : base(UserId, Pwd, Code, TokenClientId, TokenClientSecret, TokenRedirectUrl)
        {
        }

        #endregion

        public string cc()
        {
            string sql = $"select id from account where name = '{5111813113822}'";
            string id = sqlSelectOutId(sql);



            return CustomerInterfaceSelect();
        }




    }
}
