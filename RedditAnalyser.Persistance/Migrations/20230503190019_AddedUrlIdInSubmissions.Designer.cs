﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RedditAnalyzer.Persistence;

#nullable disable

namespace RedditAnalyzer.Persistence.Migrations
{
    [DbContext(typeof(RedditAnalyzerDbContext))]
    [Migration("20230503190019_AddedUrlIdInSubmissions")]
    partial class AddedUrlIdInSubmissions
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("RedditAnalyzer.Domain.Entities.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("RedditAnalyzer.Domain.Entities.Submission", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDead")
                        .HasColumnType("bit");

                    b.Property<Guid>("SubredditId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UrlId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("SubredditId");

                    b.HasIndex("UrlId")
                        .IsUnique();

                    b.ToTable("Submissions");
                });

            modelBuilder.Entity("RedditAnalyzer.Domain.Entities.Subreddit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Subreddits");
                });

            modelBuilder.Entity("RedditAnalyzer.Domain.Entities.Url", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Urls");
                });

            modelBuilder.Entity("RedditAnalyzer.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("RedditAnalyzer.Domain.Entities.Comment", b =>
                {
                    b.HasOne("RedditAnalyzer.Domain.Entities.Submission", "Submission")
                        .WithMany("Comments")
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RedditAnalyzer.Domain.Entities.User", "User")
                        .WithMany("Comments")
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Submission");

                    b.Navigation("User");
                });

            modelBuilder.Entity("RedditAnalyzer.Domain.Entities.Submission", b =>
                {
                    b.HasOne("RedditAnalyzer.Domain.Entities.User", "Creator")
                        .WithMany("Submissions")
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RedditAnalyzer.Domain.Entities.Subreddit", "Subreddit")
                        .WithMany("Submissions")
                        .HasForeignKey("SubredditId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RedditAnalyzer.Domain.Entities.Url", "Url")
                        .WithOne("Submission")
                        .HasForeignKey("RedditAnalyzer.Domain.Entities.Submission", "UrlId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Creator");

                    b.Navigation("Subreddit");

                    b.Navigation("Url");
                });

            modelBuilder.Entity("RedditAnalyzer.Domain.Entities.Submission", b =>
                {
                    b.Navigation("Comments");
                });

            modelBuilder.Entity("RedditAnalyzer.Domain.Entities.Subreddit", b =>
                {
                    b.Navigation("Submissions");
                });

            modelBuilder.Entity("RedditAnalyzer.Domain.Entities.Url", b =>
                {
                    b.Navigation("Submission");
                });

            modelBuilder.Entity("RedditAnalyzer.Domain.Entities.User", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Submissions");
                });
#pragma warning restore 612, 618
        }
    }
}
