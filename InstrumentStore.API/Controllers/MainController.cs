using InstrumentStore.API.Contracts;
using InstrumentStore.Application.Services;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class MainCintroller : ControllerBase
    {
        //private readonly InstrumentServices _instrumentServices;

        //public MainCintroller()
        //{
        //    _instrumentServices = new InstrumentServices();
        //}

        //[HttpGet]
        //public ActionResult<List<InstrumentResponse>> GetInstruments()
        //{
        //    List<Product> instruments = _instrumentServices.GetAll();

        //    IEnumerable<InstrumentResponse> response = instruments.Select(x => new InstrumentResponse(
        //        x.ProductId,
        //        x.Name,
        //        x.Description,
        //        x.Price,
        //        x.Quantity,
        //        x.Image,
        //        x.Type,
        //        x.CountryId,
        //        x.BrandId));

        //    return Ok(response);
        //}

        //[HttpGet("{Id:int}")]
        //public ActionResult<InstrumentResponse> GetInstrument(int Id)
        //{
        //    Product instrument = _instrumentServices.Get(Id);

        //    InstrumentResponse response = new InstrumentResponse(
        //        instrument.ProductId,
        //        instrument.Name,
        //        instrument.Description,
        //        instrument.Price,
        //        instrument.Quantity,
        //        instrument.Image,
        //        instrument.Type,
        //        instrument.CountryId,
        //        instrument.BrandId);

        //    return Ok(response);
        //}

        //[HttpPost]
        //public ActionResult<int> CreateInstrument([FromBody] InstrumentRequest instrument)
        //{
        //    Product inst = new Product(
        //        0,
        //        instrument.Name,
        //        instrument.Description,
        //        instrument.Price,
        //        instrument.Quantity,
        //        instrument.Image,
        //        instrument.InstrumentType,
        //        instrument.Country,
        //        instrument.Supplier);

        //    return Ok(_instrumentServices.Create(inst));
        //}

        //[HttpPut("{Id:int}")]
        //public ActionResult<int> UpdateInstrument(int Id, [FromBody] InstrumentRequest instrument)
        //{
        //    int instrumentId = _instrumentServices.Update(
        //        Id,
        //        instrument.Name,
        //        instrument.Description,
        //        instrument.Price,
        //        instrument.Quantity,
        //        instrument.Image,
        //        instrument.InstrumentType,
        //        instrument.Country,
        //        instrument.Supplier);

        //    return Ok(instrumentId);
        //}

        //[HttpDelete("{Id:int}")]
        //public ActionResult<int> DeleteInstrument(int Id)
        //{
        //    return Ok(_instrumentServices.Delete(Id));
        //}
    }
}
