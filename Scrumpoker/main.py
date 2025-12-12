from dotenv import load_dotenv
import os
import paramiko

# Load local .env
load_dotenv()

host = os.getenv("SSH_HOST")
user = os.getenv("SSH_USER")
password = os.getenv("SSH_PASS")

cmd = "cd ~/scripts && ./deploy.sh"

ssh = paramiko.SSHClient()
ssh.set_missing_host_key_policy(paramiko.AutoAddPolicy())
ssh.connect(hostname=host, username=user, password=password)

stdin, stdout, stderr = ssh.exec_command(cmd)

print(stdout.read().decode())
print(stderr.read().decode())

ssh.close()
