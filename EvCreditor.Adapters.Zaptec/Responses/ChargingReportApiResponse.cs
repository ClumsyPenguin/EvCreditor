namespace EvCreditor.Adapters.Zaptec.Responses;

public record ChargingReportApiResponse(
    string InstallationName,
    string InstallationAddress,
    string InstallationZipCode,
    string InstallationCity,
    string InstallationTimeZone,
    string GroupedBy,
    DateTime Fromdate,
    DateTime Enddate,
    List<TotalUserChargerReportModel> totalUserChargerReportModel
);

public record TotalUserChargerReportModel(
    double TotalChargeSessionCount,
    double TotalChargeSessionEnergy,
    double TotalChargeSessionDuration
);
