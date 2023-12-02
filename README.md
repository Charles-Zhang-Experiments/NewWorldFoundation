# New World Foundation (NWF)

Tags: NFMS

To serve more reliable and progressive construction of a new world (virtual digital world with digital assets and projects), I take data integrity very seriously, especially concerning unreliability of SSD storage and cloud services. I thus provide a heavily opinionated whole system backup scheme offering reliable, fast and cheap solution to data backup and integrity problems leveraging the best tools possible for fastest and most efficient experience. On the simplest level, this is a manual backup tool replacing iDrive/Backblaze for local-based bacup and works as compliment to OneDrive. This complies to NFMS but should provide a generic framework for configuration.

On the more advanced level, this tool provide robust indexing and in the future provide direct cloud backup integration through plugins.

Features:

|Feature|Function|
|-|-|
|Manual Backup|Starts Incremental Backup and Folder Structure Mirroring; Instantly see exactly what will be changed before anything happens|
|NFMS Viewer|Provide unified views at arbitrary roots|

Notice back up is not enough, to provide resilliency, it's recommended that we distribute storage of data according to their usage type/access frequency, so even if one disk fails, others still stands.

## Specification

Behavior of entry problem is configured through a single YAML configuration file.

Example:

```yaml
Mirrors:
	- Master: C:\Projects
	  Copy: 
```

## Components

Each individual component shall be used as Pure library or as CLI tool.

## Abstraction & Concepts

To the tool, it just sees disk locations, and maybe network locations (through drivers), and it provides manipulation for those. It identifies a Master and a Copy, where the copy will deligently mirror the exact content and structure of master, under a root.
