variable "do_token" {
  description = "Digital Ocean API Token"
  sensitive   = true
}

variable "ssh_key_fingerprint" {
  description = "SSH Key Fingerprint for Droplet access"
}

variable "environment" {
  description = "Environment (dev, staging, prod)"
  default     = "dev"
}
