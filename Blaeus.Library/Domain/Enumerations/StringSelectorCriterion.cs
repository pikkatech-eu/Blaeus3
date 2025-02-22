/***********************************************************************************
* File:         ComparisonKind.cs                                                  *
* Contents:     Enum ComparisonKind                                                *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2024-09-12 11:50                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

namespace Blaeus.Library.Domain.Enumerations
{
    /// <summary>
    /// Describes how toponyms are compared with the names of localities on a data source.
    /// Can be fully supported only on a local database.
    /// </summary>
    public enum StringSelectorCriterion
    {
        /// <summary>
		/// Selects strings that contain the probe string.
        /// SQL: LIKE statement ("WHERE `name` LIKE '%{token}%'"),
        /// as in "SELECT * FROM locality WHERE  `name` LIKE '%est%' ORDER BY `population`".
        /// This comparison is case-insensitive in SQL.
        /// </summary>
        ProbeIn,

        /// <summary>
		/// Selects strings that are contained in the probe string.
        /// SQL: INSTR statement ("WHERE INSTR(`name`, {token})>0"),
        /// as in "SELECT * FROM locality WHERE  INSTR('pTesto', `name`) > 0 ORDER BY `population`".
        /// This comparison is case-sensitive in SQL.
        /// </summary>
        ProbeContains,

        /// <summary>
		/// Selects strings the SOUNDEX value of which is equal to the SOUNDEX value of the probe string.
        /// SQL: SOUNDEX function is built-in ("WHERE SOUNDEX({`name`} = SOUNDEX({token}))").
        /// This comparison is case-insensitive in SQL.
        /// </summary>
        Soundex,

        /// <summary>
		/// Selects strings on the basis of Daitch-Mokotoff comparison method. 
		/// The method has a few versions which are reflected in the static properties of the Alison.Daimox class.
        /// SQL: Daitch-Mokotoff Soundex using the DAIMOK function ("WHERE DAIMOK({`name`} = DAIMOK({token}))").
        /// The DAIMOK function is still to implement and is not part of the MVP 1.0 version.
        /// </summary>
        Daimok,

        /// <summary>
		/// Selects strings that are similar with the probe string due to the cosine similarity criterion. 
		/// This method depends on the vectorizer uzed, and the value of similarity threshold.
        /// SQL: Cosine similarity ("WHERE COSINR_SIMILARITY(`name`, {token}) > {threshold}").
        /// The SIMILARITY function is still to define and implement and is not part of the MVP 1.0 version.
        /// </summary>
        Cosine,

		/// <summary>
		/// Selects strings that are similar with the probe string due to the Needleman-Wunsch similarity criterion. 
		/// This method depends on the value of similarity threshold.
		/// </summary>
		NeedleWish
    }
}
