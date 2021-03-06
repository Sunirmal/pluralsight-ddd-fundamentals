﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using BlazorShared.Models.Patient;
using ClinicManagement.Core.Aggregates;
using ClinicManagement.Core.Specifications;
using Microsoft.AspNetCore.Mvc;
using PluralsightDdd.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace ClinicManagement.Api.PatientEndpoints
{
  public class List : BaseAsyncEndpoint
    .WithRequest<ListPatientRequest>
    .WithResponse<ListPatientResponse>
  {
    private readonly IRepository _repository;
    private readonly IMapper _mapper;

    public List(IRepository repository, IMapper mapper)
    {
      _repository = repository;
      _mapper = mapper;
    }

    [HttpGet("api/patients")]
    [SwaggerOperation(
        Summary = "List Patients",
        Description = "List Patients",
        OperationId = "patients.List",
        Tags = new[] { "PatientEndpoints" })
    ]
    public override async Task<ActionResult<ListPatientResponse>> HandleAsync([FromQuery] ListPatientRequest request, CancellationToken cancellationToken)
    {
      var response = new ListPatientResponse(request.CorrelationId());

      var spec = new ClientByIdIncludePatientsSpecification(request.ClientId);
      var client = await _repository.GetAsync<Client, int>(spec);
      if (client == null) return NotFound();

      response.Patients = _mapper.Map<List<PatientDto>>(client.Patients);
      response.Count = response.Patients.Count;

      return Ok(response);
    }
  }
}
