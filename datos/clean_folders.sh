#!/bin/bash

# Script to recursively remove obj, bin directories and enunciado.md files from TP directory
# Created by GitHub Copilot

echo "Starting cleanup of TP directory..."

# Find and remove all bin and obj directories
find ../TP -type d -name "obj" -o -name "bin" | while read dir; do
  echo "Removing directory: $dir"
  rm -rf "$dir"
done

# Find and remove all enunciado.md files
find ../TP -type f -name "enunciado.md" | while read file; do
  echo "Removing file: $file"
  rm -f "$file"
done

echo "Cleanup complete!"