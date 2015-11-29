
using HeyVoteClassLibrary.Helper;
using HeyVoteClassLibrary.Managers;
using libphonenumber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeyVoteClassLibrary.Validation
{
    public class HeyVoteUtil
    {
        public List<ContactInfo> ValidatePhoneNumbers(List<ContactInfo> lstContacts, string defaultContryCode, List<TerritoryInfo> lstTerritory)
        {
            try
            {
                List<ContactInfo> lstFilteresContacts = new List<ContactInfo>();
                PhoneNumberUtil phoneUtil = PhoneNumberUtil.Instance;
                TerritoryManager mgr = new TerritoryManager();

                lstContacts.ForEach(x =>
                {
                    try
                    {
                        PhoneNumber num = CheckBasicCountryCode(x.Number, phoneUtil, defaultContryCode);
                        if (num.IsValidNumber && (num.NumberType == PhoneNumberUtil.PhoneNumberType.MOBILE || num.NumberType == PhoneNumberUtil.PhoneNumberType.FIXED_LINE_OR_MOBILE))
                        {
                            if (num.CountryCode != null)
                            {
                                x.TerritoryId = mgr.GetTerritoryIdByCountryCode(String.Format("+{0}", num.CountryCode), lstTerritory).Id;
                                x.Number = num.NationalNumber.ToString();
                                var info = lstFilteresContacts.Where(y => y.Number.Equals(x.Number)).FirstOrDefault();
                                if (info == null)
                                    lstFilteresContacts.Add(x);

                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        if (!ex.Message.Equals(CodeHelper.ContactNumberNotValid))
                            throw ex;
                    }
                });

                return lstFilteresContacts;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.ContactNumberNotValid);
            }

        }

        public PhoneNumber CheckBasicCountryCode(string number, PhoneNumberUtil util, string countryCode)
        {
            try
            {
                return util.Parse(number, String.Empty);
            }
            catch (Exception)
            {
                try
                {
                    return util.Parse(String.Format("{0}{1}", countryCode, number), "");
                }
                catch (Exception)
                {
                    throw new Exception(CodeHelper.ContactNumberNotValid);
                }
            }
        }

    }
}
