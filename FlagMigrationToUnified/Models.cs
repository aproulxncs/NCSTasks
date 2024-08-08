using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlagMigrationToUnified
{
    public class BaseFlag
    {
        //public string alert_id { get; set; }
        public string flag_id { get; set; }
        public string location_id { get; set; }
        public string company_id { get; set; }
        public string organization_id { get; set; }
        public string table_name { get; set; }
        public string table_id { get; set; } // AKA transaction_id
        public string flagged { get; set; }
        public string blue_box { get; set; }
        public string bound { get; set; }
        public string tier { get; set; }
        public string flagging_period_start { get; set; }
        public string flagging_period_end { get; set; }
        public string created_at { get; set; }
        public string display { get; set; }
        public string assessment_flags_id { get; set; }
        public string LegacyTransactionId { get; set; }
        public string LegacyAssessmentFlagId { get; set; }
    }
    public class AssessmentFlagsCultivation : BaseFlag
    {
        public string harvest_name { get; set; }
        public string unit_of_measure { get; set; }
        public string harvest_start_date { get; set; }
        public string harvest_finish_date { get; set; }
        public string harvest_plant_count { get; set; }
        public string total_wet_weight { get; set; }
        public string total_waste_weight { get; set; }
        public string total_package_weight { get; set; }
        public string total_trim_weight { get; set; }
        public string current_weight { get; set; }
        public string wet_weight_per_plant { get; set; }
        public string trim_weight_ratio { get; set; }
        public string cure_ratio { get; set; }
        public string waste_weight_ratio { get; set; }
        public string flowering_time { get; set; }
        public string vegetative_time { get; set; }
        public string grow_time { get; set; }
        public string cure_time { get; set; }
        public string planted_date { get; set; }
        public string vegetative_date { get; set; }
        public string flowering_date { get; set; }
        public string harvested_date { get; set; }
        public string plant_batch_name { get; set; }
        public string plant_batch_count { get; set; }
        public string untracked_count { get; set; }
        public string tracked_count { get; set; }
        public string packaged_count { get; set; }
        public string destroyed_count { get; set; }
        public string last_modified { get; set; }
        
    }

    public class LegacyAssessmentFlag
    {
        public string flag_id { get; set; }
        public string location_id { get; set; }
        public string company_id { get; set; }
        public string organization_id { get; set; }
        public string table_name { get; set; }
        public string table_id { get; set; } // AKA transaction_id
        public string blue_box { get; set; }
        public string created_at { get; set; }
        public string LegacyTransactionId { get; set; }
        public string LegacyAssessmentFlagId { get; set; }
        public string value_name_1 { get; set; }
        public string value_prop_1 { get; set; }
        public string value_name_2 { get; set; }
        public string value_prop_2 { get; set; }
        public string value_name_3 { get; set; }
        public string value_prop_3 { get; set; }
        public string value_name_4 { get; set; }
        public string value_prop_4 { get; set; }
        public string value_name_5 { get; set; }
        public string value_prop_5 { get; set; }
        public string value_name_6 { get; set; }
        public string value_prop_6 { get; set; }
        public string value_name_7 { get; set; }
        public string value_prop_7 { get; set; }
        public string value_name_8 { get; set; }
        public string value_prop_8 { get; set; }
        public string value_name_9 { get; set; }
        public string value_prop_9 { get; set; }
    }
}
