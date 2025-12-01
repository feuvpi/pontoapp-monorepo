terraform {
  required_providers {
    digitalocean = {
      source = "digitalocean/digitalocean"
      version = "~> 2.0"
    }
  }
}

provider "digitalocean" {
  token = var.do_token
}

resource "digitalocean_droplet" "ponto_app" {
  image  = "ubuntu-22-04-x64"
  name   = "ponto-app-${var.environment}"
  region = "nyc1"  # Escolha a região mais próxima dos seus usuários
  size   = "s-2vcpu-4gb"
  ssh_keys = [var.ssh_key_fingerprint]

  tags = ["ponto-app", var.environment]
}

resource "digitalocean_volume" "postgres_data" {
  region                  = "nyc1"
  name                    = "postgres-data-${var.environment}"
  size                    = 10
  initial_filesystem_type = "ext4"
  description             = "PostgreSQL data volume"
}

resource "digitalocean_volume_attachment" "postgres_data_attachment" {
  droplet_id = digitalocean_droplet.ponto_app.id
  volume_id  = digitalocean_volume.postgres_data.id
}

output "droplet_ip" {
  value = digitalocean_droplet.ponto_app.ipv4_address
}
