﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace VideovigilanciaDB.Models.VideovigilanciaBCP;

public partial class AlarmaAnalitica
{
    public int Id { get; set; }

    public int? AlarmEventId { get; set; }

    public string AlarmInCode { get; set; }

    public int? IdDeviceDomainCode { get; set; }

    public int? AlarmInType { get; set; }

    public string AlarmInName { get; set; }

    public int? AlarmLevelValue { get; set; }

    public string AlarmLevelName { get; set; }

    public string AlarmLevelColor { get; set; }

    public string AlarmType { get; set; }

    public string AlarmTypeName { get; set; }

    public string AlarmCategory { get; set; }

    public DateTime? OccurTime { get; set; }

    public int? OccurNumber { get; set; }

    public int? AlarmStatus { get; set; }

    public int? IsCommission { get; set; }

    public string PreviewUrl { get; set; }

    public int? ExistsRecord { get; set; }

    public string NvrCode { get; set; }

    public string Reserve { get; set; }

    public string AlarmDesc { get; set; }

    public string ExtParam { get; set; }
}