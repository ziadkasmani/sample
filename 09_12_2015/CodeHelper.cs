using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeyVoteClassLibrary.Helper
{
    public class CodeHelper
    {

        public const string UnableToCreateDirectory = "Unable to create directory";

        public const string UnableToAddUser = "Unable to add user";

        public const string UnableToDeleteDirectory = "Unable to delete directory";

        public const string UnableToCheckUserInfo = "Unable to check user info";

        public const string CustomPicFileExtension = ".jpg";

        public const string CustomVideoFileExtension = ".mp4";

        public const string CustomAudioFileExtension = ".mp3";

        #region Acces Keys

        public const string SecretKey = "@#$H%^&";

        public const string SecretAccessKey = "@#$H##^&";

        public const string SecretAccessKeyPayload = "fg4532652";

        public const string HeaderAccessKey = "hjtyu34";

        #endregion

        #region Token Error Messages

        public const string InvalidToken = "T0";

        public const string InvalidLoginToken = "T0";

        public const string InvalidHeader = "T0";

        #endregion

        #region File Error Messages

        public const string UnableToAddFile = "Unable to add file";

        #endregion

        #region User Column Names

        public const string Idf = "Idf";

        public const string FolderPath = "FolderPath";

        public const string TerritoryId = "TerritoryId";

        public const string CountryCode = "CountryCode";

        #endregion

        #region Post Error Messages

        public const string PublishErrorMesage = "500";

        public const string UnableToSchedulePost = "501";

        public const string UnableToGetPostList = "502";

        public const string UnableToGetPostDetails = "506";

        public const string UnableToVote = "503";

        public const string PostResultsDeclared = "504";

        public const string UnableToAddPost = "505";

        public const string UnableToSpamPost = "506";

        public const string UnableToSubscribePost = "507";

        public const string UnableToUnSubscribePost = "508";

        public const string UnableToReHeyVotePost = "509";

        public const string UnableToDeletePost = "510";

        #endregion

        #region Encode / Decode Error Mesages

        public const string UnableToEncode = "T0";

        public const string UnableToDecode = "T0";

        #endregion

        #region User Error Messages

        public const string UnableToGetUserBasicInfo = "101";

        public const string UnableToUpdateStatus = "102";

        public const string UnableToUpdateName = "103";

        public const string UnableToUpdateProfilePicture = "104";

        public const string UnableToViewProfile = "105";

        public const string UnableToBlockUser = "106";

        public const string UnableToUnBlockUser = "107";

        public const string UnableToFollowUser = "108";

        public const string UnableToUnFollowUser = "109";

        #endregion

        #region Folder Error Messages

        public const string UnableToFindFolder = "301";

        public const string UnableToFindFile = "301";

        #endregion

        #region Contact Error Messages

        public const string UnableToFetchContacts = "401";

        public const string UnableToAddPostContacts = "402";

        public const string UnableToAddContact = "403";

        public const string UnableToRefreshContacts = "407";

        public const string ContactNumberNotValid = "408";

        public const string UnableToSyncContacts = "409";

        #endregion

        #region Cookie Error Messages

        public const string UnableToGetCookie = "T0";

        #endregion

        #region Territory Error Messages

        public const string UnableToGetTerritoryList = "701";

        public const string UnableToFindTerritory = "702";

        public const string UnableToFindUserTerritory = "703";

        #endregion

        #region Category Error Messages

        public const string UnableToGetCategoryList = "1101";

        #endregion

        #region Comment Error Messages

        public const string UnableToComment = "801";

        public const string UnableToDeleteComment = "802";

        public const string UnableToEditComment = "803";

        public const string UnableToGetComment = "804";

        #endregion

        #region Notification Error Messages

        public const string UnableToGetNotifications = "901";

        #endregion

        #region OTP Error Messages

        public const string UnableToGenerateOTP = "1201";

        #endregion



    }
}
