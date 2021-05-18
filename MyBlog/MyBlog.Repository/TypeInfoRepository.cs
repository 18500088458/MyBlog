using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MyBlog.IRepository;
using MyBlog.Model;

namespace MyBlog.Repository
{
    public class TypeInfoRepository:BaseRepository<TypeInfo>,ITypeInfoRepository
    {
    }
}
