﻿namespace Licensing.Keys
{
    public class KeyGenerationRequestBody
    {
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Label) && 
                   !string.IsNullOrWhiteSpace(CustomerId) && 
                   !string.IsNullOrWhiteSpace(SiteId) && 
                   !string.IsNullOrWhiteSpace(CreatedBy) && 
                   !string.IsNullOrWhiteSpace(UpdatedBy);
        }

        public string? Label { get; set; }
        public string? CustomerId { get; set; }
        public string? SiteId { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? Description { get; set; }
    }

    public class KeyUpdateRequestBody
    {
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Label) &&
                   !string.IsNullOrWhiteSpace(CustomerId) &&
                   !string.IsNullOrWhiteSpace(SiteId) &&
                   !string.IsNullOrWhiteSpace(UpdatedBy);
        }

        public string? Label { get; set; }
        public string? CustomerId { get; set; }
        public string? SiteId { get; set; }
        public string? UpdatedBy { get; set; }
        public string? Description { get; set; }
    }
}