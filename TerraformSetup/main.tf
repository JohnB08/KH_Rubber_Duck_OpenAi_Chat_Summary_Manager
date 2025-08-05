
terraform {
  required_providers {
    hcloud = {
      source = "hetznercloud/hcloud"
      version = "~> 1.45"
    }
  }
}

provider "hcloud" {
  token = var.hcloud_token
}

resource "hcloud_ssh_key" "default" {
  name = "rubberduck_ssh_key"
  public_key = file("~/.ssh/id_ed25519.pub")
}

resource "hcloud_server" "rubberduckserver" {
  name = "web-server"
  image = "ubuntu-22.04"
  server_type = "cx32"
  location = "fsn1"
  ssh_keys = [hcloud_ssh_key.default.id]
  provisioner "remote-exec" {
    inline = [
      "sudo apt-get update",
      "sudo apt-get install -y git docker.io docker-compose",
      "sudo usermod -aG docker $USER"
    ]
  }
  connection {
    type = "ssh"
    user = "root"
    private_key = file("~/.ssh/id_ed25519")
    host = self.ipv4_address
  }
}