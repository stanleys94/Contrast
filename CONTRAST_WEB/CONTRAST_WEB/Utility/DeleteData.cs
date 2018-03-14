using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace CONTRAST_WEB.Models
{
    public class DeleteData
    {
        //public static async Task TravelDitDel(int? id)
        //{
        //    //tb_r_travel_actualcost UpdatedData = model;
        //    tb_r_travel_dit updated = await GetData.TravelDitList(id);
        //    updated.active_flag = false;
        //    updated.comments = "deleted";

        //    using (var client = new HttpClient())
        //    {
        //        //Passing service base url  
        //        client.BaseAddress = new Uri(Constant.Baseurl);

        //        client.DefaultRequestHeaders.Clear();
        //        //Define request data format  
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //        //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
        //        //HttpResponseMessage response = await client.PutAsJsonAsync("api/ActualCost/" + UpdatedData.id_actualcost, UpdatedData);
        //        HttpResponseMessage response = await client.PutAsync("api/TravelDIT/" + updated.id_dit, new StringContent(
        //                       new JavaScriptSerializer().Serialize(updated), Encoding.UTF8, "application/json"));
        //    }
        //}


    }
}