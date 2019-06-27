
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Common.ExceptionHandling;
using Siemens.Simatic.Platform.Data;

using Siemens.Simatic.Util.Common;
using Siemens.Simatic.Util.DataAccess;

namespace Siemens.Simatic.Util.BusinessLogic.DefaultImpl
{
    public class Plat_util_CodeBaseBO : IPlat_util_CodeBaseBO
    {
        private IPlat_util_CodeBaseDAO plat_util_CodeBaseDAO;

        public Plat_util_CodeBaseBO()
        {
            plat_util_CodeBaseDAO = ObjectContainer.BuildUp<IPlat_util_CodeBaseDAO>();
        }

        #region base interface impl

        public Plat_util_CodeBase Insert(Plat_util_CodeBase entity)
        {
            Plat_util_CodeBase newEntity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "Plat_util_CodeBase Entity");

                newEntity = plat_util_CodeBaseDAO.Insert(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return newEntity;
        }

        public void Delete(Plat_util_CodeBase entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "Plat_util_CodeBase Entity");

                plat_util_CodeBaseDAO.Delete(entity.CodeBaseGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void Delete(Guid entityGuid)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entityGuid, "Plat_util_CodeBase Guid");

                plat_util_CodeBaseDAO.Delete(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void Update(Plat_util_CodeBase entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "Plat_util_CodeBase Entity");

                plat_util_CodeBaseDAO.Update(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void UpdateSome(Plat_util_CodeBase entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "Plat_util_CodeBase Entity");

                plat_util_CodeBaseDAO.Update(entity, false);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public Plat_util_CodeBase GetEntity(Guid entityGuid)
        {
            Plat_util_CodeBase entity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entityGuid, "Plat_util_CodeBase Guid");

                entity = plat_util_CodeBaseDAO.Get(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entity;
        }

        public IList<Plat_util_CodeBase> GetAll()
        {
            long totalRecords = 0;
            IList<Plat_util_CodeBase> entities = null;

            try
            {
                entities = plat_util_CodeBaseDAO.Find(0, -1, null, null, out totalRecords);
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