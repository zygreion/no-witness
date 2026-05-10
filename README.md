# No Witness

Project ini menggunakan Unity Editor versi 2022.3.62f3

## ⚙️ Cloning Project Tutorial

1. Buka Unity Hub
2. Add project dengan sumber dari repository

   <img width="auto" height="400" alt="Screenshot_19" src="https://github.com/user-attachments/assets/dbf26ad1-4c60-421c-8431-e1e4684aa0a9" />

4. Ubah source control menjadi GitHub
5. Pastikan akun GitHub sudah terhubung ke Unity
6. Pilih `zygreion/no-witness` sebagai sumber repository

   <img width="auto" height="350" alt="Screenshot_20" src="https://github.com/user-attachments/assets/f036f88b-f6c9-42a9-bffa-e3ca932d2285" />

> ⚠️ Jika ada asset seperti gambar yang error saat kloning project, lakukan `git lfs pull` dan refresh file explorer di dalam Unity Editor


## 📚 Branching Tutorial

1. Di sini, pembuatan branch baru merujuk kepada branch `development`. Buka branch tersebut terlebih dahulu

   <img width="auto" height="60" alt="image" src="https://github.com/user-attachments/assets/84b366d9-643d-455b-b3fb-47a40c40b4e3" />

2. Buat branch baru yang bersumber dari branch `development` agar tidak terjadi tabrakan modifikasi kode antaranggota

   <img width="auto" height="45" alt="image" src="https://github.com/user-attachments/assets/78384429-08ea-4ef3-809a-d3e6c9b8932f" />

3. Setelah melakukan modifikasi project seperti penambahan asset, lakukan `git add .` dan `git commit -m "..."` dengan pesan commit yang sesuai

   <img width="auto" height="200" alt="modification" src="https://github.com/user-attachments/assets/91fe19f8-050e-4ab4-ac2e-d92975a521ab" />

4. Push perubahan branch ke GitHub

   <img width="auto" height="350" alt="Screenshot_24" src="https://github.com/user-attachments/assets/2610654b-42de-48d3-a2da-725dffe4fdde" />

> ⚠️ Modifikasi pada branch `main | development` hanya bisa dilakukan melalui pull-request


## Alur Versioning

Di sini, branch `main` berfokus kepada hasil akhir (atau banyak perubahan) yang sudah bersih dari konflik.
Sementara, branch `development` merupakan tempat bergabungnya branch-branch lain seperti fitur.

1. Push fitur yang telah dibuat misalnya `feature/tilemap`
2. Compare & pull request `development <- feature/tilemap`
3. Jika perubahan pada branch development dirasa sudah cukup, kita bisa langsung Compare & pull request `main <- development`
4. Branch selain `feature/tilemap` yang memerlukan perubahan dapat mengambil data terbaru dari `development`
