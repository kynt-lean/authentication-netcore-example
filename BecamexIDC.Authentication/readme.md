Ghi chú lỗi:
401: bạn chưa đăng nhập vào hệ thống
403: bạn không có quyền truy cập chức năng này

Phân quyền theo Role thì để vào phía đâu của Controller với Tên role

// có 2 cách phân policy 
áp dụng cho từng route cụ thể hoặc cho cả Controller
1 Role với Policy
Phân quyền với RolePolicy  được lưu trog RolePermission Class
2. User với Policy
Phân quyền với UserPolicy  được lưu trog UserPermission Class

Cấu hình tham khảo trong PolicyInstaller.cs

```
# start docker
docker run -it --name xlnt -p 3333:3333 -p 6789:6789 -v /run/media/nemo/DATA/git/xlnt:/src mcr.microsoft.com/dotnet/core/sdk:3.1
```