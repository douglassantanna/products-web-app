using Flurl.Http;
using Flurl.Http.Testing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using product.Domain.Omie.OmieCustomers.Requests;
using products.Domain.Customers.Commands;
using products.Domain.Customers.Handlers;
using products.Domain.Customers.Interfaces;
using products.Domain.Infra.Context;
using products.Domain.Infra.Repositories.CustomerRepo;
using products.Domain.Omie;
using products.Domain.Omie.OmieCustomers;
using Xunit.Abstractions;

namespace products.Domain.IntegrationTests.Customer_Test;

public class CriarNovoCliente_Teste
{
    private readonly AppDbContext _db;
    private readonly ICustomerRepository _customerRepository;
    private readonly ITestOutputHelper _helper;
    NewShippingAddress _validAddress;
    List<NewShippingAddress> _validAddresses;
    Mock<IMediator> _mediator = new Mock<IMediator>();

    Mock<IOmieCustomer> _omieCustomer = new Mock<IOmieCustomer>();
    Mock<ILogger<OmieGetCustomerHandler>> _loggerGetCustomerHandler = new Mock<ILogger<OmieGetCustomerHandler>>();
    private readonly HttpTest _httpTest;

    public CriarNovoCliente_Teste(ITestOutputHelper helper)
    {
        _validAddress = new(
                entEndereco: "rua teste",
                entNumero: "12",
                entComplemento: "complemento teste",
                entBairro: "bairro teste",
                entCEP: "13219110",
                entEstado: "SP",
                entCidade: "Jundiaí"
        );
        _validAddresses = new List<NewShippingAddress>();
        _validAddresses.Add(_validAddress);
        var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase("dshop")
        .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
        .Options;
        _db = new AppDbContext(options);
        _customerRepository = new CustomerRepository(_db);
        _mediator.Setup(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()));
        _helper = helper;
        _httpTest = new HttpTest();
    }

    [Fact]
    public async void AoCriarUmCliente_ComDadosCorretos_DeveSerPersistido()
    {
        var _loggerCreateCustomerHandler = new Mock<ILogger<CreateCustomerCommandHandler>>();
        CreateCustomerCommand command = new(
            Email: "douglas@teste.com",
            Razao_social: "douglas teste",
            Nome_fantasia: "douglas teste",
            Cnpj_cpf: "40560278896",
            Contato: "douglas teste",
            Telefone1_ddd: "11",
            Telefone1_numero: "941012994",
            Endereco: "rua teste",
            Endereco_numero: "12",
            Bairro: "bairro teste",
            Complemento: "complemento teste",
            Estado: "SP",
            Cidade: "Jundiaí",
            Cep: "13219110",
            Contribuinte: "S",
            Observacao: "",
            Pessoa_fisica: "S",
            EnderecoEntrega: _validAddresses
        );
        CreateCustomerValidator validator = new();
        var validation = validator.Validate(command);
        Assert.True(validation.IsValid);

        CreateCustomerCommandHandler handler = new(_customerRepository, _mediator.Object, _loggerCreateCustomerHandler.Object, _omieCustomer.Object);
        var result = await handler.Handle(command, default(CancellationToken));
        Assert.True(result.Success);

        _mediator.Verify(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async void AoObterUmCliente_ComDadosCorretos_DeveSerPersistido()
    {
        OmieGetCustomerRequest command = new(
                        codigo_cliente_omie: 4966714673,
                        codigo_cliente_integracao: "57.351.558/0001-39"
        );

        OmieGetCustomerHandler handler = new(_omieCustomer.Object, _loggerGetCustomerHandler.Object);
        var result = await handler.Handle(command, default(CancellationToken));
        Assert.True(result.Success);
    }
    [Fact]
    public async void AoObterUmClienteNaOmie_ComDadosValidos_RetornarStatusCode200()
    {
        _httpTest.ForCallsTo("https://app.omie.com.br/api/v1/geral/clientes/").AllowRealHttp();

        OmieGetCustomerRequest body = new(
                        codigo_cliente_omie: 1931381211,
                        codigo_cliente_integracao: "65.654.303/0001-73"
        );
        OmieGeneralRequest omieRequest = new(
            call: "ConsultarCliente",
                    app_key: "2699300300697",
                    app_secrets: "b7ab98a7fc57e3aba0639bcbf393ff39",
                    new List<object>()
                    {
                        body
                    });

        var result = await "https://app.omie.com.br/api/v1/geral/clientes/"
                .WithHeader("Content-type", "application/json")
                .WithHeader("accept", "application/json")
                .PostJsonAsync(omieRequest);

        var statusCodeResult = result.StatusCode;
        Assert.Equal(200, statusCodeResult);
    }
}