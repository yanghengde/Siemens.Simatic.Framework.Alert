using System;
using System.Collections.Generic;
using System.Text;

namespace Siemens.Simatic.Service.AlertService
{
    public interface IService
    {
        string Name { get; }
        bool DoService();
        void Start();
        void Stop();
    }
}
