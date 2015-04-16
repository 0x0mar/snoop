#include <stdlib.h>
#include <stdio.h>

#include <pcap.h>

#define MAX_PRINT 80
#define MAX_LINE 16

void usage();

int main(int argc, char **argv)
{
	pcap_t *fp;
	pcap_if_t *alldevs, *d;
	u_int inum = 0;
	char errbuf[PCAP_ERRBUF_SIZE];
	char *source = NULL;
	char *ofilename = NULL;
	char *filter = NULL;
	int i = 0;
	pcap_dumper_t *dumpfile;
	struct bpf_program fcode;
	bpf_u_int32 NetMask;
	int res;
	struct pcap_pkthdr *header;
	const u_char *pkt_data;

	if (argc == 1)
	{
		usage();
		return -1;
	}

	printf("\nPrinting the device list:\n");
	/* The user didn't provide a packet source: Retrieve the local device list */
	if (pcap_findalldevs_ex(PCAP_SRC_IF_STRING, NULL, &alldevs, errbuf) == -1)
	{
		fprintf(stderr, "Error in pcap_findalldevs_ex: %s\n", errbuf);
		getchar();
		return -1;
	}

	/* Print the list */
	for (d = alldevs; d; d = d->next)
	{
		printf("%d. %s\n    ", ++i, d->name);
		if (d->description)
			printf(" (%s)\n", d->description);
		else
			printf(" (No description available)\n");
	}

	if (i == 0)
	{
		fprintf(stderr, "No interfaces found! Exiting.\n");
		getchar();
		return -1;
	}

	printf("Enter the interface number (1-%d):", i);
	scanf_s("%d", &inum);

	if (inum < 1 || inum > i)
	{
		printf("\nInterface number out of range.\n");

		/* Free the device list */
		pcap_freealldevs(alldevs);
		getchar();
		return -1;
	}

	/* Jump to the selected adapter */
	for (d = alldevs, i = 0; i< inum - 1; d = d->next, i++);

	/* Open the device */
	if ((fp = pcap_open(d->name,
		100 /*snaplen*/,
		PCAP_OPENFLAG_PROMISCUOUS /*flags*/,
		20 /*read timeout*/,
		NULL /* remote authentication */,
		errbuf)
		) == NULL)
	{
		fprintf(stderr, "\nError opening adapter\n");
		getchar();
		return -1;
	}

	// parse the filename and filter string
	for (i = 1; i < argc; i += 2)
	{
		switch (argv[i][1])
		{
			case 'o':
			{
				ofilename = argv[i + 1];
			};
			break;
			case 'f':
			{
				filter = argv[i + 1];
			};
			break;
		}
	}

	if (filter != NULL)
	{
		// We should loop through the adapters returned by the pcap_findalldevs_ex()
		// in order to locate the correct one.
		//
		// Let's do things simpler: we suppose to be in a C class network ;-)
		NetMask = 0xffffff;
		//compile the filter
		if (pcap_compile(fp, &fcode, filter, 1, NetMask) < 0)
		{
			fprintf(stderr, "\nError compiling filter: wrong syntax.\n");
			getchar();
			return -1;
		}
		//set the filter
		if (pcap_setfilter(fp, &fcode)<0)
		{
			fprintf(stderr, "\nError setting the filter\n");
			getchar();
			return -1;
		}
	}

	//open the dump file
	if (ofilename != NULL)
	{
		dumpfile = pcap_dump_open(fp, ofilename);
		if (dumpfile == NULL)
		{
			fprintf(stderr, "\nError opening output file\n");
			getchar();
			return -1;
		}
	}
	else usage();

	//start the capture
	while ((res = pcap_next_ex(fp, &header, &pkt_data)) >= 0)
	{
		if (res == 0)
			/* Timeout elapsed */
			continue;

		//save the packet on the dump file
		pcap_dump((unsigned char *)dumpfile, header, pkt_data);
	}
}


void usage()
{
	printf("\npf - Generic Packet Filter.\n");
	printf("\nUsage:\npf -o output_file_name [-f filter_string]\n\n");
	getchar();
	exit(0);
}