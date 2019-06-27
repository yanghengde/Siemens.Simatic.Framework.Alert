
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Common.ExceptionHandling;
using Siemens.Simatic.Platform.Data;

using Siemens.Simatic.ALT.Common;
using Siemens.Simatic.ALT.DataAccess;

namespace Siemens.Simatic.ALT.BusinessLogic.DefaultImpl
{
    public class PM_ALT_CONFIG_KEYBO : IPM_ALT_CONFIG_KEYBO
    {
        private IPM_ALT_CONFIG_KEYDAO pM_ALT_CONFIG_KEYDAO;

        public PM_ALT_CONFIG_KEYBO()
        {
            pM_ALT_CONFIG_KEYDAO = ObjectContainer.BuildUp<IPM_ALT_CONFIG_KEYDAO>();
        }

        #region base interface impl

        public PM_ALT_CONFIG_KEY Insert(PM_ALT_CONFIG_KEY entity)
        {
            PM_ALT_CONFIG_KEY newEntity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_CONFIG_KEY Entity");

                newEntity = pM_ALT_CONFIG_KEYDAO.Insert(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return newEntity;
        }

        public void Delete(PM_ALT_CONFIG_KEY entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_CONFIG_KEY Entity");

                pM_ALT_CONFIG_KEYDAO.Delete(entity.ID);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void Delete(Int32 entityID)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entityID, "PM_ALT_CONFIG_KEY Int32");

                pM_ALT_CONFIG_KEYDAO.Delete(entityID);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void Update(PM_ALT_CONFIG_KEY entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_CONFIG_KEY Entity");

                pM_ALT_CONFIG_KEYDAO.Update(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void UpdateSome(PM_ALT_CONFIG_KEY entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_CONFIG_KEY Entity");

                pM_ALT_CONFIG_KEYDAO.Update(entity, false);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public PM_ALT_CONFIG_KEY GetEntity(Int32 entityID)
        {
            PM_ALT_CONFIG_KEY entity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entityID, "PM_ALT_CONFIG_KEY Int32");

                entity = pM_ALT_CONFIG_KEYDAO.Get(entityID);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entity;
        }

        public IList<PM_ALT_CONFIG_KEY> GetAll()
        {
            long totalRecords = 0;
            IList<PM_ALT_CONFIG_KEY> entities = null;

            try
            {
                entities = pM_ALT_CONFIG_KEYDAO.Find(0, -1, null, null, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        #endregion base interface impl

    }
}