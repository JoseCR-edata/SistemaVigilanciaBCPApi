﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using APIBCPSistemaVigilancia.Models.BDBCPSistemaVigilancia;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace APIBCPSistemaVigilancia.Models.BDBCPSistemaVigilancia
{
    public partial class BCPSistemaVigilanciaContext
    {
        private IBCPSistemaVigilanciaContextProcedures _procedures;

        public virtual IBCPSistemaVigilanciaContextProcedures Procedures
        {
            get
            {
                if (_procedures is null) _procedures = new BCPSistemaVigilanciaContextProcedures(this);
                return _procedures;
            }
            set
            {
                _procedures = value;
            }
        }

        public IBCPSistemaVigilanciaContextProcedures GetProcedures()
        {
            return Procedures;
        }

        protected void OnModelCreatingGeneratedProcedures(ModelBuilder modelBuilder)
        {
        }
    }

    public partial class BCPSistemaVigilanciaContextProcedures : IBCPSistemaVigilanciaContextProcedures
    {
        private readonly BCPSistemaVigilanciaContext _context;

        public BCPSistemaVigilanciaContextProcedures(BCPSistemaVigilanciaContext context)
        {
            _context = context;
        }

        public virtual async Task<int> AgregarUsuarioConectadoAsync(string Usuario, DateTime? FechaIngreso, string Ip, DateTime? UltimaConsulta, string Grupo, bool? Estado, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default)
        {
            var parameterreturnValue = new SqlParameter
            {
                ParameterName = "returnValue",
                Direction = System.Data.ParameterDirection.Output,
                SqlDbType = System.Data.SqlDbType.Int,
            };

            var sqlParameters = new []
            {
                new SqlParameter
                {
                    ParameterName = "Usuario",
                    Size = 50,
                    Value = Usuario ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.VarChar,
                },
                new SqlParameter
                {
                    ParameterName = "FechaIngreso",
                    Value = FechaIngreso ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.DateTime,
                },
                new SqlParameter
                {
                    ParameterName = "Ip",
                    Size = 50,
                    Value = Ip ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.VarChar,
                },
                new SqlParameter
                {
                    ParameterName = "UltimaConsulta",
                    Value = UltimaConsulta ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.DateTime,
                },
                new SqlParameter
                {
                    ParameterName = "Grupo",
                    Size = 50,
                    Value = Grupo ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.VarChar,
                },
                new SqlParameter
                {
                    ParameterName = "Estado",
                    Value = Estado ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.Bit,
                },
                parameterreturnValue,
            };
            var _ = await _context.Database.ExecuteSqlRawAsync("EXEC @returnValue = [dbo].[AgregarUsuarioConectado] @Usuario, @FechaIngreso, @Ip, @UltimaConsulta, @Grupo, @Estado", sqlParameters, cancellationToken);

            returnValue?.SetValue(parameterreturnValue.Value);

            return _;
        }

        public virtual async Task<int> ModificarUsuarioConectadoAsync(string Usuario, DateTime? FechaIngreso, string Ip, DateTime? UltimaConsulta, string Grupo, bool? Estado, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default)
        {
            var parameterreturnValue = new SqlParameter
            {
                ParameterName = "returnValue",
                Direction = System.Data.ParameterDirection.Output,
                SqlDbType = System.Data.SqlDbType.Int,
            };

            var sqlParameters = new []
            {
                new SqlParameter
                {
                    ParameterName = "Usuario",
                    Size = 50,
                    Value = Usuario ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.VarChar,
                },
                new SqlParameter
                {
                    ParameterName = "FechaIngreso",
                    Value = FechaIngreso ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.DateTime,
                },
                new SqlParameter
                {
                    ParameterName = "Ip",
                    Size = 50,
                    Value = Ip ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.VarChar,
                },
                new SqlParameter
                {
                    ParameterName = "UltimaConsulta",
                    Value = UltimaConsulta ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.DateTime,
                },
                new SqlParameter
                {
                    ParameterName = "Grupo",
                    Size = 50,
                    Value = Grupo ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.VarChar,
                },
                new SqlParameter
                {
                    ParameterName = "Estado",
                    Value = Estado ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.Bit,
                },
                parameterreturnValue,
            };
            var _ = await _context.Database.ExecuteSqlRawAsync("EXEC @returnValue = [dbo].[ModificarUsuarioConectado] @Usuario, @FechaIngreso, @Ip, @UltimaConsulta, @Grupo, @Estado", sqlParameters, cancellationToken);

            returnValue?.SetValue(parameterreturnValue.Value);

            return _;
        }
    }
}
