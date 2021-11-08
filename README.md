# submerge

A simple CLI tool to help with releasing features in monorepository. It creates a branch based on difference between base and head for a directory. It is an obvious overkill for single project single repo but in case you have several different projects you need to release using GitOps it can eliminate manual work.

## Workflow
Given you're in the git repo and you need to merge a specific directory of repo from **dev** to **master**, the tool automates the next flow:
```console
git checkout master
cd path/to/dir
git log --oneline

git checkout dev
git log -- online
```
then taking the difference between two branches and cherry-picking missing commits onto **master**

## Usage
```bash
submerge [options]
```

| Option | Description | Type | Required? |
| ------------ | -------------------------------------------------- | -------- | --------- |
| `-b, --base` | base branch for which a new branch will be created | `string` | Yes |
| `-h, --head` | branch which will be compared with a base branch | `string` | Yes |
| `-n, --name` | name of the result branch | `string` | Yes |
