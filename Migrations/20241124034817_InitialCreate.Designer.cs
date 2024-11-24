﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AchievementsPlatform.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241124034817_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AccountGame", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AppId")
                        .HasColumnType("integer");

                    b.Property<string>("IconUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("PlaytimeForever")
                        .HasColumnType("integer");

                    b.Property<string>("SteamUserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("TotalAchievements")
                        .HasColumnType("integer");

                    b.Property<int?>("UserAchievements")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("AccountGames");
                });

            modelBuilder.Entity("Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SteamId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("TotalGames")
                        .HasColumnType("integer");

                    b.Property<int>("TotalHoursPlayed")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("SteamAchievement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("Achieved")
                        .HasColumnType("boolean");

                    b.Property<string>("ApiName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("GameName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("PlayerId")
                        .HasColumnType("integer");

                    b.Property<string>("SteamId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("UnlockDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.ToTable("SteamAchievements");
                });

            modelBuilder.Entity("SteamAchievement", b =>
                {
                    b.HasOne("Player", null)
                        .WithMany("SteamAchievements")
                        .HasForeignKey("PlayerId");
                });

            modelBuilder.Entity("Player", b =>
                {
                    b.Navigation("SteamAchievements");
                });
#pragma warning restore 612, 618
        }
    }
}
