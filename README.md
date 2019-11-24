# Spire
A Very Simple Automation/Command & Control Solution

# Overview
Spire was designed to be flexible, extensible, fast and also easy to set up, use and understand.
It is extremely portable because it is fully confined to a single directory.
For configuration, it uses a combination of YAML and plaintext files.

Spire consists of two programs: server-update and slave.

# server-update
Server-update takes two arguments:

Markup: 1. Slave group which to update
		2. Name of the script file which to run on the slaves

All slaves, along with groups which they belong to, are specified in a <i>slavelist.yaml</i> file.

# slavelist.yaml
Slavelist.yaml file is parsed by server-update in order to determine which slaves to send a POST request to.
The file is organized as a list of groups, each having a name and a list of slaves which belong to that group.

For example:

'''yaml
---
groups:
 - name:	Local
   slaves:
    - ip:	http://127.0.0.1
    - ip:	http://localhost

 - name:	Remote
   slaves:
    - ip:	http://example.com
...
'''

It is possible to, for example, run a script called example.sh only on the Local group by running:

'''bash
./server-update Local example.sh
'''

Alternatively, it is possible to, instead of specifying a group name, tell the program to run a certain script on all groups like so:

'''bash
./server-update all example.sh
'''

The above command will tell all of the slaves specified in the slavelist.yaml file to search for and execute <i>example.sh</i> on their local machines.

# slave
Slave is a web server which listens on port 10000.
Slave can send and receive data only through a POST request. The data it receives is a script name which it then looks for in the Scripts folder, located within the same directory as the slave program itself. If it finds the script in question, it will attempt to execute it within a separate process. If it succeeds, it will notify the sender that the script was executed successfully; Alternatively, it will notify the sender that something went wrong or that the script in question was not found.

It is important to note that all scripts should have a proper #! line, along with executable permissions. If they do not, the slave catch and log an error.

# secret.txt
<i>Secret.txt</i> file should be filled with arbitrary, random data supplied by the user. It is used for secure hashing of data which is to be transmitted over the network. It should be located within the same directory as the programs.

Server-update transmits data in a form of a SHA256 hash. When a script name is specified, for example <i>example.sh</i>, the name is concatenated with the contents of the <i>secret.txt</i> file.
A SHA256 hash is then made out of the aforementioned combination and sent to the slave via a POST request.

The slave receives the SHA256 hash. It then reads its own secret.txt file and begins going through all of the scripts in the Scripts folder. It concatenates the contents of its own <i>secret.txt</i> file with each script name it finds in the Scripts folder. If it finds a SHA256 hash that matches the one sent to it via the POST request, it knows that it is the script which it should execute.

<b><i>For this exchange to work, secret.txt files should be identical on both the server-update side and on the slave side.</i></b> If they are not, the slave will never find a matching hash and will, therefore, notify the sender that it was unable to find the script in question.

# tl;dr

The files needed for server-update are:
Markup: 1. slavelist.yaml
		2. secret.txt

The files and folders needed for slave are:
Markup: 1. Scripts (folder)
		2. Script files within the Scripts folder (With proper permissions and #! lines)
		3. secret.txt

Both secret.txt files on the server-update side and slave side should be identical.
All files should be located within the same directory as the programs are.