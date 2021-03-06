﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Caching;
using M2E.Models;
using System.Configuration;
using M2E.Encryption;
using M2E.Common.Logger;
using System.Reflection;
using M2E.CommonMethods;
using System.Data.Entity.Validation;

namespace M2E.Session
{
    public class TokenManager
    {
        private static readonly ILogger Logger = new Logger(Convert.ToString(MethodBase.GetCurrentMethod().DeclaringType));
        private DbContextException _dbContextException = new DbContextException();
        private readonly M2EContext _db = new M2EContext();

        public static void CreateSession(M2ESession session)
        {
            var sessionId = session.SessionId;
            const int hours = 1; // TODO: currently hard coded hour value;
            //MemoryCache.Default.Set(sessionId, session, new CacheItemPolicy() { SlidingExpiration = new TimeSpan(hours, 0, 0) });
            setMemoryCacheValue(sessionId, session, hours, 0, 0);
        }

        private static void setMemoryCacheValue(string SessionId, M2ESession session, int hours, int minutes, int seconds)
        {
            MemoryCache.Default.Set(SessionId, session, new CacheItemPolicy() { SlidingExpiration = new TimeSpan(hours, 0, 0) });
        }
        public static void RemoveSession(string sessionId)
        {
            MemoryCache.Default.Remove(sessionId);
        }

        public static string GetUsernameFromSessionId(HeaderManager headers)
        {
            var session = getSessionInfo(headers.AuthToken, headers);
            if (session != null)
                return session.UserName;
            else
                return null;
        }

        public static void UpdateSignalRClientAddr(M2ESession session,dynamic signalRClientAddr)
        {
            session.SignalRClient = signalRClientAddr;
            const int hours = 1; // TODO: currently hard coded hour value;
            setMemoryCacheValue(session.SessionId, session, hours, 0, 0);
        }

        public static M2ESession getLogoutSessionInfo(string sessionId)
        {
            M2ESession session = null;
            if (IsValidSession(sessionId, out session))
            {
                return session;
            }
            return null;
        }

        public static M2ESession getSessionInfo(string sessionId, HeaderManager headers)
        {
            M2ESession session = null;
            if (IsValidSession(sessionId, out session))
            {
                return session;
            }
            else
            {
                if (headers == null)
                    return null;
                if (sessionId == null)
                    return null;
                string Authkey = ConfigurationManager.AppSettings["AuthKey"];                

                string username = EncryptionClass.GetDecryptionValue(headers.AuthKey, Authkey);
                M2EContext _db = new M2EContext();
                var dbUserInfo = _db.Users.SingleOrDefault(x=>x.Username == username);
                if (dbUserInfo != null)
                {
                    var data = new Dictionary<string, string>();                    
                    data["Password"] = headers.AuthValue;
                    data["userGuid"] = dbUserInfo.guid;

                    try
                    {
                        var decryptedData = EncryptionClass.decryptUserDetails(data);

                        if (dbUserInfo.KeepMeSignedIn == "true" && dbUserInfo.Password == decryptedData["UTMZV"])
                        {
                            var NewSession = new M2ESession(username, sessionId);
                            TokenManager.CreateSession(NewSession);
                            return getSessionInfo(sessionId, headers);
                        }
                        else
                        {
                            return null;
                        }

                    }
                    catch (Exception)
                    {

                        return null;
                    }
                    

                }
                else
                {
                    return null;
                }                
            }
        }

        public static M2ESession getSessionInfo(string sessionId)
        {
            M2ESession session = null;
            if (IsValidSession(sessionId, out session))
            {
                return session;
            }
            else
            {
                return null;
            }
        }
        

        public static bool IsValidSession(string sessionId)
        {
            if (sessionId == null)
                return false;

            M2ESession session = null;
            return IsValidSession(sessionId, out session);
        }

        public bool Logout(string sessionId)
        {
            if (sessionId == null)
                return false;

            M2ESession session = null;
            if (MemoryCache.Default.Contains(sessionId))
            {
                session = (M2ESession)MemoryCache.Default.Get(sessionId);
                var user = _db.Users.SingleOrDefault(x => x.Username == session.UserName);
                user.guid = Guid.NewGuid().ToString();
                try
                {
                    _db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    DbContextException.LogDbContextException(e);                    
                }

                MemoryCache.Default.Remove(sessionId);
            }
            return true;
        }

        private static bool IsValidSession(string sessionId, out  M2ESession session)
        {
            session = null;

            if (sessionId == null)
                return false;
            if (MemoryCache.Default.Contains(sessionId))
            {
                session = (M2ESession)MemoryCache.Default.Get(sessionId);                
            }
            return VerifySessionObject(session);
        }

        private static bool VerifySessionObject(M2ESession session)
        {
            return session != null;
        }
    }
}