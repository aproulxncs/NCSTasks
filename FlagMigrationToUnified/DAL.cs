using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace FlagMigrationToUnified
{
    public class DAL
    {
        string cs = "server=ld6zibm9l36m5f.ct0nlj8qzmco.us-west-2.rds.amazonaws.com; user id=sqldevadmin; pwd=HTK<8Xj8`4iA; database=devdb";

        public async Task MigrateFlagDetailsToUnified()
        {
            

            string sqlText = @"
                SELECT
	                f.id LegacyFlagId,
	                f.flag_id, 
	                f.location_id,
	                f.company_id,
	                f.organization_id,
                    f.flagged,
	                t.id LegacyTransactionId,
	                t.table_id,
	                t.table_name,
	                t.created_at TransactionCreatedAt,
	                t.magnitude,
	                d.*
                FROM assessment_flags f
                JOIN assessment_metadata m on f.flag_id = m.flag_id
                JOIN assessment_alert_transactions t on t.alert_id = f.id
                JOIN assessment_alert_details d on d.transaction_id = t.id
                WHERE m.active = 1
	                AND f.[month] IN (1,2,3)
	                AND f.[year] = 2024
	                AND m.phase = 'retail'
                ORDER BY d.transaction_id, d.column_number;
            ";


            var res = new List<AssessmentFlagsRetail>();

            // Object created using connection string
            using (var connection = new SqlConnection(cs))
            {
                try
                {
                    connection.Open();

                    using (var sqlCommand = new SqlCommand(sqlText, connection))
                    {
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.CommandTimeout = 600;
                        
                        Console.WriteLine($"Attempting to execute flag migration....");

                        using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                        {
                            var curTransactionId = "";
                            LegacyAssessmentFlag curRow = null;

                            while (reader.Read())
                            {
                                var transactionId = reader["LegacyTransactionId"].ToString();
                                if (curTransactionId != transactionId)
                                {
                                    //
                                    // Add previous row to result list.
                                    // curRow is only null on start.
                                    if (curRow != null)
                                    {
                                        res.Add(curRow);
                                    }

                                    
                                    curTransactionId = transactionId;
                                    curRow = new LegacyAssessmentFlag();

                                    curRow.flag_id = reader["flag_id"].ToString();
                                    curRow.location_id = reader["location_id"].ToString();
                                    curRow.company_id = reader["company_id"].ToString();
                                    curRow.organization_id = reader["organization_id"].ToString();
                                    curRow.table_name = reader["table_name"].ToString();
                                    curRow.table_id = reader["table_id"].ToString();
                                    curRow.created_at = reader["TransactionCreatedAt"].ToString();
                                    curRow.LegacyAssessmentFlagId = reader["LegacyFlagId"].ToString();
                                    curRow.LegacyTransactionId = curTransactionId;


                                    MapValueNameAndProp(reader, curRow, (int)reader["column_number"]);
                                }
                                else
                                {
                                    MapValueNameAndProp(reader, curRow, (int)reader["column_number"]);

                                    if (reader["blue"] != DBNull.Value && (int)reader["blue"] == 1)
                                    {
                                        curRow.blue_box = reader["value_char"].ToString()?.Replace("$", string.Empty).Replace(",", string.Empty);
                                    }
                                }
                            } // END WHILE
                        }

                    } // END COMMAND

                    Console.WriteLine($"Legacy Flags Serialized.....");
                    Console.WriteLine($"Attempting to execute MERGE Stored Procedure.....");

                    var dt = ToDataTable(res);

                    using (var sqlCommand = new SqlCommand())
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.CommandText = "flag.MigrateLegacyRetailAlerts";
                        sqlCommand.Connection = connection;
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@legacyFlags";
                        param.SqlDbType = SqlDbType.Structured;
                        param.Value = dt;
                        param.TypeName = "flag.assessment_flags_type";
                        sqlCommand.Parameters.Add(param);
                        sqlCommand.ExecuteNonQuery();

                    }

                    Console.WriteLine($"Flag migration executed SUCCESSFULLY");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"FAILED to execute flag migration: {ex.Message}");
                    connection.Close();
                    throw;
                }
            } // END CONNECTION
        } // END METHOD

        private string GetValueProp(SqlDataReader reader)
        {
            return reader["value_date"] == DBNull.Value ? reader["value_char"].ToString() : reader["value_date"].ToString();
        }

        private void MapValueNameAndProp(SqlDataReader reader, LegacyAssessmentFlag curRow, int colNum)
        {
            switch (colNum)
            {
                case 1:
                    curRow.value_name_1 = reader["value_name"].ToString();
                    curRow.value_prop_1 = GetValueProp(reader);
                    break;

                case 2:
                    curRow.value_name_2 = reader["value_name"].ToString();
                    curRow.value_prop_2 = GetValueProp(reader);
                    break;

                case 3:
                    curRow.value_name_3 = reader["value_name"].ToString();
                    curRow.value_prop_3 = GetValueProp(reader);
                    break;

                case 4:
                    curRow.value_name_4 = reader["value_name"].ToString();
                    curRow.value_prop_4 = GetValueProp(reader);
                    break;

                case 5:
                    curRow.value_name_5 = reader["value_name"].ToString();
                    curRow.value_prop_5 = GetValueProp(reader);
                    break;

                case 6:
                    curRow.value_name_6 = reader["value_name"].ToString();
                    curRow.value_prop_6 = GetValueProp(reader);
                    break;

                case 7:
                    curRow.value_name_7 = reader["value_name"].ToString();
                    curRow.value_prop_7 = GetValueProp(reader);
                    break;

                case 8:
                    curRow.value_name_8 = reader["value_name"].ToString();
                    curRow.value_prop_8 = GetValueProp(reader);
                    break;

                case 9:
                    curRow.value_name_9 = reader["value_name"].ToString();
                    curRow.value_prop_9 = GetValueProp(reader);
                    break;


                default:
                    break;
            }
        }

        private void MapCultivationValueName(string valueName, string valueString, string valueDate, string flagged, AssessmentFlagsCultivation curRow)
        {
            switch (valueName)
            {
                case "Affected Plants": curRow.plant_batch_count = valueString; break;

                case "Blue Box Cure Ratio": curRow.blue_box = valueString; break;
                case "Blue Box Cure Time (Days)": curRow.blue_box = valueString; break;
                case "Blue Box Flowering Time (Days)": curRow.blue_box = valueString; break;
                case "Blue Box Fresh Plant Ratio": curRow.blue_box = valueString; break;
                case "Blue Box Grow Time (Days)": curRow.blue_box = valueString; break;
                case "Blue Box Plant Batch Count": curRow.blue_box = valueString; break;
                case "Blue Box Waste Weight Ratio": curRow.blue_box = valueString; break;
                case "Blue Box Wet Weight Per Plant (Grams)": curRow.blue_box = valueString; break;

                case "Cure Ratio": curRow.cure_ratio = valueString; break;
                case "Cure Time (Days)": curRow.cure_time = valueString; break;
                case "Destroyed Count": curRow.destroyed_count = valueString; break;
                //case "Excess Weight (Grams)": 
                //case "Flagged Plant Count": curRow.
                case "Flowering Date": curRow.flowering_date = valueDate; break;
                case "Flowering Time (Days)": curRow.flowering_time = valueString; break;
                //case "Fresh Plant Ratio": 
                case "Grow Time (Days)": curRow.grow_time = valueString; break;
                case "Growth Time (Days)": curRow.grow_time = valueString; break;
                case "Harvest Date": curRow.harvested_date = valueDate; break;
                case "Harvest Finish Date": curRow.harvest_finish_date = valueDate; break;
                case "Harvest Finished Date": curRow.harvest_finish_date = valueDate; break;
                case "Harvest Lot Name": curRow.harvest_name = valueString; break;
                case "Harvest Name": curRow.harvest_name = valueString; break;
                case "Harvest Start Date": curRow.harvest_start_date = valueDate; break;
                //case "Harvest Type": 
                case "Harvested Count": curRow.harvest_plant_count = valueString; break;
                case "Harvested Date": curRow.harvested_date = valueDate; break;
                case "Harvested Start Date": curRow.harvest_start_date = valueDate; break;
                //case "Initial Plant Count":
                case "Last Modified": curRow.last_modified = valueDate; break;
                case "Last Modified Date": curRow.last_modified = valueDate; break;
                case "Live Count": curRow.plant_batch_count = valueString; break; // ??
                case "Live Plant Count": curRow.plant_batch_count = valueString; break; // ??
                //case "Month": 
                case "Packaged Count": curRow.packaged_count = valueString; break;
                case "Plant Batch Name": curRow.plant_batch_name = valueString; break;
                case "Plant Count": curRow.plant_batch_count = valueString; break; // ??
                //case "Plant Label": 
                case "Planted Date": curRow.planted_date = valueDate; break;
                //case "Total Dry Weight (Grams)": 
                //case "Total Fresh Plant Weight (Grams)": 
                case "Total Plant Count": curRow.plant_batch_count = valueString; break; // ??
                case "Total Trim Weight (Grams)": curRow.total_trim_weight = valueString; break;
                case "Total Waste Weight (Grams)": curRow.total_waste_weight = valueString; break;
                case "Total Wet Weight (Grams)": curRow.total_wet_weight = valueString; break;
                case "Tracked Count": curRow.tracked_count = valueString; break;
                case "Untracked Count": curRow.untracked_count = valueString; break;
                case "Vegetative Date": curRow.vegetative_date = valueString; break;
                case "Waste Weight Ratio": curRow.waste_weight_ratio = valueString; break;
                case "Wet Weight Per Plant (Grams)": curRow.wet_weight_per_plant = valueString; break;
            }

        }

        private void MapRetailValueName(string valueName, string valueString, string valueDate, AssessmentFlagsRetail curRow)
        {
            switch (valueName)
            {
                case "Blue Box Price Per Gram": curRow.Blue_Box = valueString.Replace(",", string.Empty); break;
                case "Blue Box Price Per Unit": curRow.Blue_Box = valueString.Replace(",", string.Empty); break;
                case "Date": curRow.sales_date_time = valueDate; break;
                case "End Date": curRow.End_Date = valueDate; break;
                case "Giveaway Count": curRow.sale_count = valueString.Replace(",", string.Empty); break; // ??
                case "Giveaway Percent": curRow.Giveaway_Percent = valueString.Replace("%", string.Empty); break;
                case "Package Quantity (each)": curRow.package_quantity = valueString.Replace(",", string.Empty); break;
                case "Package Quantity (grams)": curRow.package_quantity = valueString.Replace(",", string.Empty); break;
                case "Price Per Gram": curRow.Price_Per_Unit_Ratio = valueString.Replace("$", string.Empty).Replace(",", string.Empty); break;
                case "Price Per Unit": curRow.Price_Per_Unit_Ratio = valueString.Replace("$", string.Empty).Replace(",", string.Empty); break;
                case "Product Name": curRow.product_name = valueString; break;
                case "Quantity Sold (each)": curRow.total_quantity = valueString.Replace(",", string.Empty); break;
                case "Quantity Sold (grams)": curRow.total_quantity = valueString.Replace(",", string.Empty); break;
                case "Receipt Number": curRow.receipt_number = valueString; break;
                case "Sale Type": curRow.sale_type = valueString; break;
                case "Start Date": curRow.Start_Date = valueDate; break;
                case "Total Price": curRow.total_price = valueString.Replace("$", string.Empty).Replace(",", string.Empty); break;
                case "Transaction Count": curRow.Transaction_Count = valueString.Replace(",", string.Empty); break;

            }
        }

        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }

            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }
}
