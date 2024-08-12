using APIBCPSistemaVigilancia.Models.BDBCPSistemaVigilancia;
using SistemaVigilanciaBCPApi.Hubs;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base;

namespace SistemaVigilanciaBCPApi.SubscribeTableDependencies
{
    public class SubscribeProductTableDependency : ISubscribeTableDependency
    {
        SqlTableDependency<UsuariosConectado> tableDependency; 
        ControlUsuarioHub controlUsuarioHub;

        public SubscribeProductTableDependency(ControlUsuarioHub controlUsuarioHub)
        {
            this.controlUsuarioHub = controlUsuarioHub;
        }   

        public void SubscribeTableDependency(string connectionString)
        {
            var mapper = new ModelToTableMapper<UsuariosConectado>();
            mapper.AddMapping(model => model.Estado, "Estado");

            tableDependency = new SqlTableDependency<UsuariosConectado>(connectionString,"UsuariosConectado","dbo",mapper);
            tableDependency.OnChanged += TableDependency_OnChanged;
            tableDependency.OnError += TableDependency_OnError;
            tableDependency.Start();
        }

        private async void TableDependency_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<UsuariosConectado> e)
        {
            if (e.ChangeType != TableDependency.SqlClient.Base.Enums.ChangeType.None)
            {
                await controlUsuarioHub.SendUsers();
            }
        }

        private void TableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
        {
            Console.WriteLine($"{nameof(UsuariosConectado)} SqlTableDependency error: {e.Error.Message}");
        }
    }
}
