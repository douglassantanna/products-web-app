using MediatR;
using Microsoft.Extensions.Logging;
using products.Domain.Omie;
using products.Domain.Omie.OmieCustomers;
using products.Domain.Shared;

namespace product.Domain.Omie.OmieCustomers.Requests;
public record OmieCreateCustomerRequest(
     string codigo_cliente_integracao,
        string email,
        string razao_social,
        string cnpj_cpf,
        string contato,
        string telefone1_numero,
        string endereco,
        string endereco_numero,
        string bairro,
        string complemento,
        string estado,
        string cidade,
        string cep,
        string contribuinte,
        string observacao,
        string pessoa_fisica
) : IRequest<NotificationResult>;

public class OmieCreateCustomerHandler : IRequestHandler<OmieCreateCustomerRequest, NotificationResult>
{
    private const string OMIE_CALL = "IncluirCliente";
    private const string APP_KEY = "2699300300697";
    private const string APP_SECRET = "b7ab98a7fc57e3aba0639bcbf393ff39";
    private readonly IOmieCustomer _omieCustomer;
    private readonly ILogger<OmieCreateCustomerHandler> _logger;

    public OmieCreateCustomerHandler(IOmieCustomer omieCustomer, ILogger<OmieCreateCustomerHandler> logger)
    {
        _omieCustomer = omieCustomer;
        _logger = logger;
    }

    public async Task<NotificationResult> Handle(OmieCreateCustomerRequest request, CancellationToken cancellationToken)
    {
        if (request is null) return new NotificationResult("Request is null");

        //create validation to OmieCreateCustomerCommand
        var body = new OmieGeneralRequest(
           call: $"{OMIE_CALL}",
           app_key: $"{APP_KEY}",
           app_secrets: $"{APP_SECRET}",
           new() { request });

        var result = await _omieCustomer.CreateCustomer(body);

        return new NotificationResult("Great job! Customer created.", true, result);
    }
}