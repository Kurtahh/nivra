using backend.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace backend.Interfaces;

public interface IStepsController
{
    IActionResult CreateStepEntry(CreateStepEntryRequest body);
}
