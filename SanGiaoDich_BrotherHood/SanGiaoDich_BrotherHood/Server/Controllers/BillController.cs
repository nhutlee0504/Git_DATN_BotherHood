﻿
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SanGiaoDich_BrotherHood.Shared.Models;
using SanGiaoDich_BrotherHood.Server.Services;
using SanGiaoDich_BrotherHood.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private IBill bill;
        public BillController(IBill bill)
        {
            this.bill = bill;
        }

        [HttpGet]
        public async Task<ActionResult> GetBills()
        {
            return Ok(await bill.GetBills());
        }

        [HttpGet]
        [Route("GetBillsByUserName/{username}")]
        public async Task<ActionResult> GetBillsByUserName(string userName)
        {
            return Ok(await bill.GetBillsByUserName(userName));
        }

        [HttpGet("IDBill")]
        public async Task<ActionResult> GetBillsByIDBill(int IDBill)
        {
            return Ok(await bill.GetBillByIDBill(IDBill));
        }

        [HttpPost]
        public async Task<ActionResult> AddBill(Bill bl)
        {
            var b = await bill.AddBill(new Bill
            {
                FullName = bl.FullName,
                PhoneNumber = bl.PhoneNumber,
                Email = bl.Email,
                DeliveryAddress = bl.DeliveryAddress,
                Total = bl.Total,
                OrderDate = bl.OrderDate,
                DateReceipt = bl.DateReceipt,
                PaymentType = bl.PaymentType,
                Status = bl.Status,
                UerName = bl.UerName
            });
            if (b == null)
                return BadRequest();
            return CreatedAtAction("AddBill",b);
        }

        [HttpPut("IDBill")]
        public async Task<ActionResult> UpdateBill(int IDBill, Bill bl)
        {
            return Ok(await bill.UpdateBill(IDBill, bl));
        }
    }
}
