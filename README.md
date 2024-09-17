## Projeto API de Autenticacao

### AutenticacaoAPIS

Este projeto é um exemplo de autenticação com ASP.NET Identity usando persistência da chave de assinatura do token em banco de dados.

Apesar de usar o banco de dados MariaDB, você pode mudar facilmente para qualquer outro banco pois usei o EntityFramework. Basta mudar a conexão no appsettings.json e o provider no IoC.cs

### Para criar o banco de dados

Execute os comandos

```powershell
 dotnet ef migrations Add CriarBanco
 dotnet ef database update
 ```

 Você pode também mudar o TipoBanco no appsettings.json para MYSQL ou SQLSERVER.

#### Atualizado para .NET 8.0


