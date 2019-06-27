
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
    public class Plat_util_CodeDetailBO : IPlat_util_CodeDetailBO
    {
        private IPlat_util_CodeDetailDAO plat_util_CodeDetailDAO;

        public Plat_util_CodeDetailBO()
        {
            plat_util_CodeDetailDAO = ObjectContainer.BuildUp<IPlat_util_CodeDetailDAO>();
        }

        #region base interface impl

        public Plat_util_CodeDetail Insert(Plat_util_CodeDetail entity)
        {
            Plat_util_CodeDetail newEntity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "Plat_util_CodeDetail Entity");

                newEntity = plat_util_CodeDetailDAO.Insert(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return newEntity;
        }

        public void Delete(Plat_util_CodeDetail entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "Plat_util_CodeDetail Entity");

                plat_util_CodeDetailDAO.Delete(entity.CodeDetailGuid);
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
                ArgumentValidator.CheckForNullArgument(entityGuid, "Plat_util_CodeDetail Guid");

                plat_util_CodeDetailDAO.Delete(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void Update(Plat_util_CodeDetail entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "Plat_util_CodeDetail Entity");

                plat_util_CodeDetailDAO.Update(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void UpdateSome(Plat_util_CodeDetail entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "Plat_util_CodeDetail Entity");

                plat_util_CodeDetailDAO.Update(entity, false);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public Plat_util_CodeDetail GetEntity(Guid entityGuid)
        {
            Plat_util_CodeDetail entity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entityGuid, "Plat_util_CodeDetail Guid");

                entity = plat_util_CodeDetailDAO.Get(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entity;
        }

        public IList<Plat_util_CodeDetail> GetAll()
        {
            long totalRecords = 0;
            IList<Plat_util_CodeDetail> entities = null;

            try
            {
                entities = plat_util_CodeDetailDAO.Find(0, -1, null, null, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        #endregion base interface impl

        public IList<Plat_util_CodeDetail> GetByCodeBase(string codeBaseCode)
        {
            long totalRecords = 0;
            IList<Plat_util_CodeDetail> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("CodeBaseCode", codeBaseCode);

                af.AddFilter(mf);

                Sort sort = new Sort();
                sort.OrderBy("MySequence", Sort.Direction.ASC);

                entities = plat_util_CodeDetailDAO.Find(0, -1, af, sort, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        public Plat_util_CodeDetail GetEntity(string codeBaseCode, string codeDetailValue)
        {
            long totalRecords = 0;
            IList<Plat_util_CodeDetail> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("CodeBaseCode", codeBaseCode);
                mf.AddMatching("CodeDetailValue", codeDetailValue);

                af.AddFilter(mf);

                entities = plat_util_CodeDetailDAO.Find(0, -1, af, null, out totalRecords);
                if (entities.Count > 0)
                    return entities[0];
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return null;
        }

    }
}