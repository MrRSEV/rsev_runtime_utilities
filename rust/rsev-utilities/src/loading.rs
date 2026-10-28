use std::{
    fs, io,
    path::{Path, PathBuf},
};
#[derive(Default)]
pub struct AssemblyLoader;
impl AssemblyLoader {
    pub fn load_directory(&self, path: impl AsRef<Path>) -> io::Result<Vec<PathBuf>> {
        Ok(fs::read_dir(path)?
            .filter_map(Result::ok)
            .map(|e| e.path())
            .filter(|p| p.is_file())
            .collect())
    }
}
#[derive(Default)]
pub struct TypeDiscovery;
impl TypeDiscovery {
    pub fn files_with_extension(
        &self,
        path: impl AsRef<Path>,
        extension: &str,
    ) -> io::Result<Vec<PathBuf>> {
        Ok(fs::read_dir(path)?
            .filter_map(Result::ok)
            .map(|e| e.path())
            .filter(|p| p.extension().and_then(|x| x.to_str()) == Some(extension))
            .collect())
    }
}
