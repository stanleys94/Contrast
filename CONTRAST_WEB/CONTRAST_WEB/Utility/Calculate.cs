using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace CONTRAST_WEB.Models
{
    public static class calculate
    {
        public static async Task<TravelRequestHelper> DateDurationAsync(TravelRequestHelper model, int model_count)
        {
            bool same_date = false;
            int week_day = 0;
            TimeSpan range = ((DateTime)model.travel_request.end_date).Date - ((DateTime)model.travel_request.start_date).Date;

            if (model_count > 0)
            {
                if (Convert.ToDateTime(model.travel_request.start_date).Date == Convert.ToDateTime(model.travel_request.end_date).Date) same_date = true;

            }
            for (int k = 0; k <= range.Days; k++)
            {
                if (Convert.ToDateTime(model.travel_request.start_date).AddDays(k).DayOfWeek == DayOfWeek.Saturday || Convert.ToDateTime(model.travel_request.start_date).AddDays(k).DayOfWeek == DayOfWeek.Sunday) week_day++;
            }

            int duration = range.Days;
            if (!same_date) duration = duration + 1;

            model.travel_request.duration = duration;

            var mealwinterallowance = await GetData.RateMealWinterInfo(model);
            model.travel_request.allowance_meal_idr = (mealwinterallowance.meal_allowance * duration) + (mealwinterallowance.meal_allowance * week_day);

            // cek winter gak?
            ///*
            if (model.travel_request.start_date.Value.Month == 12 || model.travel_request.start_date.Value.Month == 1 || model.travel_request.start_date.Value.Month == 2)
            {
                model.travel_request.allowance_winter = mealwinterallowance.winter_allowance;
            }
            else
                model.travel_request.allowance_winter = 0;

            if (model.travel_request.air_ticket_flag == true)
            {
                var rateflight = await GetData.RateFlightInfo(model);
                model.travel_request.allowance_ticket = (rateflight.economy) * 2;
            }
            else
            {
                var rateland = await GetData.Procedures(model.employee_info.@class);
                model.travel_request.allowance_ticket = (Convert.ToInt32(rateland.land_transport)) * 2;
            }

            var ratehotel = await GetData.RateHotelInfo(model);

            if (!same_date)
            {
                if (model.travel_request.overseas_flag == true) model.travel_request.allowance_hotel = ratehotel.overseas * (duration - 1);
                else
                    model.travel_request.allowance_hotel = ratehotel.domestik * (duration - 1);
            }
            else
            {
                if (model.travel_request.overseas_flag == true) model.travel_request.allowance_hotel = ratehotel.overseas * duration;
                else
                    model.travel_request.allowance_hotel = ratehotel.domestik * duration;
            }

            model.travel_request.apprv_flag_lvl1 = "0";
            model.travel_request.allowance_preparation = 0;
            model.travel_request.grand_total_allowance = (model.travel_request.allowance_meal_idr != null ? model.travel_request.allowance_meal_idr : 0) +
                                                                (model.travel_request.allowance_hotel != null ? model.travel_request.allowance_hotel : 0) +
                                                                (model.travel_request.allowance_preparation != null ? model.travel_request.allowance_preparation : 0) +
                                                                (model.travel_request.allowance_ticket != null ? model.travel_request.allowance_ticket : 0) +
                                                                (model.travel_request.allowance_winter != null ? model.travel_request.allowance_winter : 0)
                                                                ;
            return model;
        }


    }
}