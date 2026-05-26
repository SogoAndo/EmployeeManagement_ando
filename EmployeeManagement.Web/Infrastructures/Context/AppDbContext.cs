using EmployeeManagement.Web.Infrastructures.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Web.Infrastructures.Context;

/// <summary>
/// アプリケーションで利用するDbContext継承クラス
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// employeesテーブルにアクセスするプロパティ
    /// </summary>
    public DbSet<EmployeeEntity> Employees { get; set; }

    /// <summary>
    /// departmentsテーブルにアクセスするプロパティ
    /// </summary>
    public DbSet<DepartmentEntity> Departments { get; set; }

    /// <summary>
    /// usersテーブルにアクセスするプロパティ
    /// </summary>
    public DbSet<LoginUserEntity> LoginUsers { get; set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="options">
    /// データベース接続情報やログ出力設定などのオプション
    /// </param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>
    /// エンティティ同士の関係や制約を設定する
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 社員番号、部門名、ログインIDは重複しないようにする。
        modelBuilder.Entity<EmployeeEntity>()
            .HasIndex(e => e.EmployeeNo)
            .IsUnique();

        modelBuilder.Entity<DepartmentEntity>()
            .HasIndex(d => d.Name)
            .IsUnique();

        modelBuilder.Entity<LoginUserEntity>()
            .HasIndex(u => u.LoginId)
            .IsUnique();

        // PostgreSQL側の既存テーブル定義に合わせて、日時カラムの型を明示する。
        modelBuilder.Entity<EmployeeEntity>()
            .Property(e => e.HireDate)
            .HasColumnType("date");

        modelBuilder.Entity<EmployeeEntity>()
            .Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone");

        modelBuilder.Entity<EmployeeEntity>()
            .Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone");

        modelBuilder.Entity<DepartmentEntity>()
            .Property(d => d.CreatedAt)
            .HasColumnType("timestamp without time zone");

        modelBuilder.Entity<DepartmentEntity>()
            .Property(d => d.UpdatedAt)
            .HasColumnType("timestamp without time zone");

        // 社員と部門: 多対1リレーション
        modelBuilder.Entity<EmployeeEntity>()
            .HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            // 所属社員がいる部門を誤って削除できないようにする。
            .OnDelete(DeleteBehavior.Restrict);

        // 社員と登録者: 多対1リレーション
        modelBuilder.Entity<EmployeeEntity>()
            .HasOne(e => e.CreatedByUser)
            .WithMany()
            .HasForeignKey(e => e.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // 社員と更新者: 多対1リレーション
        modelBuilder.Entity<EmployeeEntity>()
            .HasOne(e => e.UpdatedByUser)
            .WithMany()
            .HasForeignKey(e => e.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // 部門と登録者: 多対1リレーション
        modelBuilder.Entity<DepartmentEntity>()
            .HasOne(d => d.CreatedByUser)
            .WithMany()
            .HasForeignKey(d => d.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // 部門と更新者: 多対1リレーション
        modelBuilder.Entity<DepartmentEntity>()
            .HasOne(d => d.UpdatedByUser)
            .WithMany()
            .HasForeignKey(d => d.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // ログインユーザーと社員: 1対1リレーション
        modelBuilder.Entity<LoginUserEntity>()
            .HasOne(u => u.Employee)
            .WithOne()
            .HasForeignKey<LoginUserEntity>(u => u.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
