using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace DbProvider.Pomocna
{
    public class HResultStatus
    {
        private Exception exception;

        public HResultStatus(Exception e, ResultStatusCodes resultStatusCodes)
        {
            this.exception = e;
            this.ResultCode = resultStatusCodes;
        }

        public ResultStatusCodes ResultCode { get; set; }
        public static string ResultMessage { get; set; }

        public enum ResultStatusCodes
        {
            FALSE = 0,
            TRUE = 1,
            ITEM_ALREADY_EXISTS = 2,
            ITEM_SUCCESS_ADD = 3,
            LOGIN_SUCCESS = 4,
            LOGIN_FAILED = 5,
            LOGIN_MISSING_DATA = 6,
            INPUT_ERROR = 7,
            ITEM_NULL = 8,
            ITEM_SUCCESS_DELETE = 9,
            ITEM_SUCCESS_UPDATE = 10,
            TRANSACTION_ERROR = 11,
            APPLICATION_ERROR = 12,
            SUCCESS = 13,
            SQL_EXCEPTION = 14
        }

        public string GetStatusMessage(ResultStatusCodes code)
        {
            ResultCode = code;
            switch (code)
            {
                case ResultStatusCodes.FALSE:
                case ResultStatusCodes.INPUT_ERROR:
                    ResultMessage = "Unos nije korektan!";
                    break;
                case ResultStatusCodes.TRUE:
                    ResultMessage = "Unos je korektan!";
                    break;
                case ResultStatusCodes.ITEM_ALREADY_EXISTS:
                    ResultMessage = "Stavka vec postoji u bazi podataka!";
                    break;
                case ResultStatusCodes.ITEM_SUCCESS_ADD:
                    ResultMessage = "Stavka uspjesno unesena u bazu podataka!";
                    break;
                case ResultStatusCodes.LOGIN_FAILED:
                    ResultMessage = "Prijava nije uspjesna!";
                    break;
                case ResultStatusCodes.LOGIN_SUCCESS:
                    ResultMessage = "Prijava je uspjesna!";
                    break;
                case ResultStatusCodes.LOGIN_MISSING_DATA:
                    ResultMessage = "Podaci za prijavu nisu korektni!";
                    break;
                case ResultStatusCodes.ITEM_NULL:
                    ResultMessage = "Podatak nije validan: ITEM_NULL";
                    break;
                case ResultStatusCodes.ITEM_SUCCESS_DELETE:
                    ResultMessage = "Stavka uspjesno uklonjena iz baze podataka!";
                    break;
                case ResultStatusCodes.ITEM_SUCCESS_UPDATE:
                    ResultMessage = "Podaci stavke uspjesno izmjenjeni u bazi podataka!";
                    break;
                case ResultStatusCodes.TRANSACTION_ERROR:
                    ResultMessage = exception.Message;
                    break;
                case ResultStatusCodes.APPLICATION_ERROR:
                    ResultMessage = exception.Message;
                    break;
                default:
                    ResultMessage = "NULL";
                    break;
            }
            return ResultMessage;
        }
    }
}
