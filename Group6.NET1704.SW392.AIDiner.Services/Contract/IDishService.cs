using Group6.NET1704.SW392.AIDiner.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group6.NET1704.SW392.AIDiner.Services.Contract
{
    public interface IDishService
    {
        public  Task<ResponseDTO> GetAllDishes();
    }
}
