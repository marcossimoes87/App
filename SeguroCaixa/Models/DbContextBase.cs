using Microsoft.EntityFrameworkCore;

namespace SeguroCaixa.Models
{
    public abstract class DbContextBase<T> : DbContext
        where T : DbContext
    {
        public DbContextBase(DbContextOptions<T> options)
            : base(options) { }

        public virtual DbSet<Sdctb001Municipio> Sdctb001Municipio { get; set; }
        public virtual DbSet<Sdctb002DocumentoCapturado> Sdctb002DocumentoCapturado { get; set; }
        public virtual DbSet<Sdctb003DocumentoExigido> Sdctb003DocumentoExigido { get; set; }
        public virtual DbSet<Sdctb004TipoLogradouro> Sdctb004TipoLogradouro { get; set; }
        public virtual DbSet<Sdctb005GrupoDocumento> Sdctb005GrupoDocumento { get; set; }
        public virtual DbSet<Sdctb006TipoDocumento> Sdctb006TipoDocumento { get; set; }
        public virtual DbSet<Sdctb007TipoIndenizacao> Sdctb007TipoIndenizacao { get; set; }
        public virtual DbSet<Sdctb008PedidoIndenizacao> Sdctb008PedidoIndenizacao { get; set; }
        public virtual DbSet<Sdctb009Pessoa> Sdctb009Pessoa { get; set; }
        public virtual DbSet<Sdctb009PessoaStg> Sdctb009PessoaStgs { get; set; }
        public virtual DbSet<Sdctb010SituacaoPedido> Sdctb010SituacaoPedido { get; set; }
        public virtual DbSet<Sdctb011TipoSituacaoPedido> Sdctb011TipoSituacaoPedido { get; set; }
        public virtual DbSet<Sdctb012TipoVeiculo> Sdctb012TipoVeiculo { get; set; }
        public virtual DbSet<Sdctb013Uf> Sdctb013Uf { get; set; }
        public virtual DbSet<Sdctb014TipoParticipacao> Sdctb014TipoParticipacao { get; set; }
        public virtual DbSet<Sdctb015Participacao> Sdctb015Participacao { get; set; }
        public virtual DbSet<Sdctb016Canal> Sdctb016Canal { get; set; }
        public virtual DbSet<Sdctb017TipoMotivoSituacao> Sdctb017TipoMotivoSituacao { get; set; }
        public virtual DbSet<Sdctb018SituacaoMotivo> Sdctb018SituacaoMotivo { get; set; }
        public virtual DbSet<Sdctb019ValorIndenizacao> Sdctb019ValorIndenizacao { get; set; }
        public virtual DbSet<Sdctb020Nacionalidade> Sdctb020Nacionalidade { get; set; }
        public virtual DbSet<Sdctb021Ocupacao> Sdctb021Ocupacao { get; set; }
        public virtual DbSet<Sdctb022Parentesco> Sdctb022Parentesco { get; set; }
        public virtual DbSet<Sdctb023DeclaraHerdeiro> Sdctb023DeclaraHerdeiro { get; set; }
        public virtual DbSet<Sdctb024SolicitanteIndenizacao> Sdctb024SolicitanteIndenizacao { get; set; }
        public virtual DbSet<Sdctb025EstadoCivil> Sdctb025EstadoCivil { get; set; }
        public virtual DbSet<Sdctb026DeclaraHerdeiroComplemento> Sdctb026DeclaraHerdeiroComplemento { get; set; }
        public virtual DbSet<Sdctb027Notificacao> Sdctb027Notificacao { get; set; }
        public virtual DbSet<Sdctb028TipoAcordo> Sdctb028TipoAcordo { get; set; }
        public virtual DbSet<Sdctb029Acordo> Sdctb029Acordo { get; set; }
        public virtual DbSet<Sdctb030DocumentoPendente> Sdctb030DocumentoPendente { get; set; }
        public virtual DbSet<Sdctb034Emitente> Sdctb034Emitentes { get; set; }
        public virtual DbSet<Sdctb035ItemDespesa> Sdctb035ItemDespesas { get; set; }
        public virtual DbSet<Sdctb036TipoItemDespesa> Sdctb036TipoItemDespesas { get; set; }
        public virtual DbSet<Sdctb044TipoIndeferimento> Sdctb044TipoIndeferimentos { get; set; }
        public virtual DbSet<Sdctb045MotivoIndeferimento> Sdctb045MotivoIndeferimentos { get; set; }

        public virtual DbSet<Sdctb046VigenciaIndenizacao> Sdctb046VigenciaIndenizacaos { get; set; }
        public virtual DbSet<Sdctb047TipoItemDocumento> Sdctb047TipoItemDocumentos { get; set; }
        public virtual DbSet<Sdctb048ItemDocumentoExigido> Sdctb048ItemDocumentoExigidos { get; set; }
        public virtual DbSet<Sdctb049ItemDocumentoConteudo> Sdctb049ItemDocumentoConteudos { get; set; }
        public virtual DbSet<Sdctb054GedamPedidoIndenizacao> Sdctb054GedamPedidoIndenizacaos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AI");

            modelBuilder.Entity<Sdctb001Municipio>(entity =>
            {
                entity.HasKey(e => e.NuMunicipio)
                    .HasName("PK_SDCTB001");

                entity.ToTable("SDCTB001_MUNICIPIO");

                entity.Property(e => e.NuMunicipio).HasColumnName("NU_MUNICIPIO");

                entity.Property(e => e.CoIbge)
                    .HasColumnType("numeric(7, 0)")
                    .HasColumnName("CO_IBGE");

                entity.Property(e => e.CoUf)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("CO_UF")
                    .IsFixedLength(true);

                entity.Property(e => e.DeMunicipio)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("DE_MUNICIPIO");

                entity.HasOne(d => d.CoUfNavigation)
                    .WithMany(p => p.Sdctb001Municipios)
                    .HasForeignKey(d => d.CoUf)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB001_SDCTB013");
            });

            modelBuilder.Entity<Sdctb002DocumentoCapturado>(entity =>
            {
                entity.HasKey(e => e.NuDocumentoCapturado)
                    .HasName("PK_SDCTB002");

                entity.ToTable("SDCTB002_DOCUMENTO_CAPTURADO");

                entity.HasIndex(e => e.NuPedidoIndenizacao, "IX_SDCTB002_1");

                entity.Property(e => e.NuDocumentoCapturado).HasColumnName("NU_DOCUMENTO_CAPTURADO");

                entity.Property(e => e.DeCaminhoBlob)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("DE_CAMINHO_BLOB");

                entity.Property(e => e.DeNomeArquivo)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("DE_NOME_ARQUIVO");

                entity.Property(e => e.DeUrlImagem)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("DE_URL_IMAGEM");

                entity.Property(e => e.DhExclusao)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_EXCLUSAO");

                entity.Property(e => e.DhInclusao)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_INCLUSAO");

                entity.Property(e => e.IcRejeitado)
                   .HasColumnType("bit")
                   .HasColumnName("IC_REJEITADO");

                entity.Property(e => e.NuDocumentoExigido).HasColumnName("NU_DOCUMENTO_EXIGIDO");

                entity.Property(e => e.NuDocumentoPendente).HasColumnName("NU_DOCUMENTO_PENDENTE");

                entity.Property(e => e.NuPagina).HasColumnName("NU_PAGINA");

                entity.Property(e => e.NuPedidoIndenizacao).HasColumnName("NU_PEDIDO_INDENIZACAO");

                entity.HasOne(d => d.NuDocumentoExigidoNavigation)
                    .WithMany(p => p.Sdctb002DocumentoCapturados)
                    .HasForeignKey(d => d.NuDocumentoExigido)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB002_SDCTB003");

                entity.HasOne(d => d.NuPedidoIndenizacaoNavigation)
                    .WithMany(p => p.Sdctb002DocumentoCapturados)
                    .HasForeignKey(d => d.NuPedidoIndenizacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB002_SDCTB008");

                entity.HasOne(d => d.NuDocumentoPendenteNavigation)
                    .WithMany(p => p.NuDocumentoCapturadoNavigation)
                    .HasForeignKey(d => d.NuDocumentoPendente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB002_SDCTB030");
            });

            modelBuilder.Entity<Sdctb003DocumentoExigido>(entity =>
            {
                entity.HasKey(e => e.NuDocumentoExigido)
                    .HasName("PK_SDCTB003");

                entity.ToTable("SDCTB003_DOCUMENTO_EXIGIDO");

                entity.Property(e => e.NuDocumentoExigido).HasColumnName("NU_DOCUMENTO_EXIGIDO");

                entity.Property(e => e.DhFim)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_FIM");

                entity.Property(e => e.DhInicio)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_INICIO");

                entity.Property(e => e.NuGrupoDocumento).HasColumnName("NU_GRUPO_DOCUMENTO");

                entity.Property(e => e.NuTipoDocumento).HasColumnName("NU_TIPO_DOCUMENTO");

                entity.HasOne(d => d.NuGrupoDocumentoNavigation)
                    .WithMany(p => p.Sdctb003DocumentoExigidos)
                    .HasForeignKey(d => d.NuGrupoDocumento)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB003_SDCTB005");

                entity.HasOne(d => d.NuTipoDocumentoNavigation)
                    .WithMany(p => p.Sdctb003DocumentoExigidos)
                    .HasForeignKey(d => d.NuTipoDocumento)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB003_SDCTB006");
            });

            modelBuilder.Entity<Sdctb004TipoLogradouro>(entity =>
            {
                entity.HasKey(e => e.SgTipoLogradouro);

                entity.ToTable("SDCTB004_TIPO_LOGRADOURO");

                entity.Property(e => e.SgTipoLogradouro)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("SG_TIPO_LOGRADOURO")
                    .IsFixedLength(true);

                entity.Property(e => e.NoTipoLogradouro)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("NO_TIPO_LOGRADOURO")
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<Sdctb005GrupoDocumento>(entity =>
            {
                entity.HasKey(e => e.NuGrupoDocumento)
                    .HasName("PK_SDCTB005");

                entity.ToTable("SDCTB005_GRUPO_DOCUMENTO");

                entity.Property(e => e.NuGrupoDocumento).HasColumnName("NU_GRUPO_DOCUMENTO");

                entity.Property(e => e.DeAbreviaturaGrupoDocumento)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("DE_ABREVIATURA_GRUPO_DOCUMENTO");

                entity.Property(e => e.DeGrupoDocumento)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("DE_GRUPO_DOCUMENTO");

                entity.Property(e => e.DhFim)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_FIM");

                entity.Property(e => e.DhInicio)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_INICIO");

                entity.Property(e => e.IcDocumentoUnico).HasColumnName("IC_DOCUMENTO_UNICO");

                entity.Property(e => e.IcObrigatorio).HasColumnName("IC_OBRIGATORIO");

                entity.Property(e => e.NuOrdem).HasColumnName("NU_ORDEM");

                entity.Property(e => e.NuTipoIndenizacao).HasColumnName("NU_TIPO_INDENIZACAO");

                entity.Property(e => e.NuTipoParticipacao).HasColumnName("NU_TIPO_PARTICIPACAO");

                entity.HasOne(d => d.NuTipoIndenizacaoNavigation)
                 .WithMany(p => p.Sdctb005GrupoDocumentos)
                 .HasForeignKey(d => d.NuTipoIndenizacao)
                 .HasConstraintName("FK_SDCTB005_SDCTB007");

                entity.HasOne(d => d.NuTipoParticipacaoNavigation)
                    .WithMany(p => p.Sdctb005GrupoDocumentos)
                    .HasForeignKey(d => d.NuTipoParticipacao)
                    .HasConstraintName("FK_SDCTB005_SDCTB014");
            });

            modelBuilder.Entity<Sdctb006TipoDocumento>(entity =>
            {
                entity.HasKey(e => e.NuTipoDocumento)
                    .HasName("PK_SDCTB006");

                entity.ToTable("SDCTB006_TIPO_DOCUMENTO");

                entity.Property(e => e.NuTipoDocumento).HasColumnName("NU_TIPO_DOCUMENTO");

                entity.Property(e => e.DeAbreviaturaTipoDocumento)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("DE_ABREVIATURA_TIPO_DOCUMENTO");

                entity.Property(e => e.DeTipoDocumento)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("DE_TIPO_DOCUMENTO");

                entity.Property(e => e.DhExclusao)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_EXCLUSAO");

                entity.Property(e => e.QtPaginas).HasColumnName("QT_PAGINAS");
            });

            modelBuilder.Entity<Sdctb007TipoIndenizacao>(entity =>
            {
                entity.HasKey(e => e.NuTipoIndenizacao)
                    .HasName("PK_SDCTB007");

                entity.ToTable("SDCTB007_TIPO_INDENIZACAO");

                entity.Property(e => e.NuTipoIndenizacao).HasColumnName("NU_TIPO_INDENIZACAO");

                entity.Property(e => e.DeAbreviaturaTipoIndenizaca)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("DE_ABREVIATURA_TIPO_INDENIZACA");

                entity.Property(e => e.DeTipoIndenizacao)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("DE_TIPO_INDENIZACAO");
            });

            modelBuilder.Entity<Sdctb008PedidoIndenizacao>(entity =>
            {
                entity.HasKey(e => e.NuPedidoIndenizacao)
                    .HasName("PK_SDCTB008");

                entity.ToTable("SDCTB008_PEDIDO_INDENIZACAO");

                entity.HasIndex(e => new { e.NuPessoaSolicitante, e.DhSinistro, e.NuTipoIndenizacao, e.NuTipoVeiculo }, "IX_SDCTB008_1");

                entity.HasIndex(e => e.NuMunicipio, "IX_SDCTB008_2");

                entity.HasIndex(e => e.NuSituacaoPedido, "IX_SDCTB008_3");

                entity.Property(e => e.NuPedidoIndenizacao).HasColumnName("NU_PEDIDO_INDENIZACAO");

                entity.Property(e => e.CoPosicaoImovel)
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasColumnName("CO_POSICAO_IMOVEL")
                    .IsFixedLength(true);

                entity.Property(e => e.DeComplemento)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("DE_COMPLEMENTO");

                entity.Property(e => e.DhPedido)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_PEDIDO");

                entity.Property(e => e.DhSinistro)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_SINISTRO");

                entity.Property(e => e.IcAutorizaConversao).HasColumnName("IC_AUTORIZA_CONVERSAO");

                entity.Property(e => e.IcAutorizaCredito).HasColumnName("IC_AUTORIZA_CREDITO");

                entity.Property(e => e.NoBairro)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("NO_BAIRRO");

                entity.Property(e => e.NoLogradouro)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("NO_LOGRADOURO");

                entity.Property(e => e.NuAgencia).HasColumnName("NU_AGENCIA");

                entity.Property(e => e.NuBanco).HasColumnName("NU_BANCO");

                entity.Property(e => e.NuCanal).HasColumnName("NU_CANAL");

                entity.Property(e => e.NuConta).HasColumnName("NU_CONTA");

                entity.Property(e => e.NuMunicipio).HasColumnName("NU_MUNICIPIO");

                entity.Property(e => e.NuPessoaSolicitante).HasColumnName("NU_PESSOA_SOLICITANTE");

                entity.Property(e => e.NuSituacaoPedido).HasColumnName("NU_SITUACAO_PEDIDO");

                entity.Property(e => e.NuTipoConta).HasColumnName("NU_TIPO_CONTA");

                entity.Property(e => e.NuTipoIndenizacao).HasColumnName("NU_TIPO_INDENIZACAO");

                entity.Property(e => e.NuTipoVeiculo).HasColumnName("NU_TIPO_VEICULO");

                entity.Property(e => e.SgTipoLogradouro)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("SG_TIPO_LOGRADOURO")
                    .IsFixedLength(true);

                entity.HasOne(d => d.NuCanalNavigation)
                    .WithMany(p => p.Sdctb008PedidoIndenizacaos)
                    .HasForeignKey(d => d.NuCanal)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB008_SDCTB016");

                entity.HasOne(d => d.NuMunicipioNavigation)
                    .WithMany(p => p.Sdctb008PedidoIndenizacaos)
                    .HasForeignKey(d => d.NuMunicipio)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB008_SDCTB001");

                entity.HasOne(d => d.NuPessoaSolicitanteNavigation)
                    .WithMany(p => p.Sdctb008PedidoIndenizacaos)
                    .HasForeignKey(d => d.NuPessoaSolicitante)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB008_SDCTB009");

                entity.HasOne(d => d.NuSituacaoPedidoNavigation)
                    .WithMany(p => p.Sdctb008PedidoIndenizacaos)
                    .HasForeignKey(d => d.NuSituacaoPedido)
                    .HasConstraintName("FK_SDCTB008_SDCTB010");

                entity.HasOne(d => d.NuTipoIndenizacaoNavigation)
                    .WithMany(p => p.Sdctb008PedidoIndenizacaos)
                    .HasForeignKey(d => d.NuTipoIndenizacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB008_SDCTB007");

                entity.HasOne(d => d.NuTipoVeiculoNavigation)
                    .WithMany(p => p.Sdctb008PedidoIndenizacaos)
                    .HasForeignKey(d => d.NuTipoVeiculo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB008_SDCTB012");

                entity.HasOne(d => d.SgTipoLogradouroNavigation)
                    .WithMany(p => p.Sdctb008PedidoIndenizacaos)
                    .HasForeignKey(d => d.SgTipoLogradouro)
                    .HasConstraintName("FK_SDCTB008_SDCTB004");
            });

            modelBuilder.Entity<Sdctb009Pessoa>(entity =>
            {
                entity.HasKey(e => e.NuPessoa)
                    .HasName("PK_SDCTB009");

                entity.ToTable("SDCTB009_PESSOA");

                entity.HasIndex(e => e.NuCpf, "IX_SDCTB009_1")
                    .IsUnique();

                entity.Property(e => e.NuPessoa).HasColumnName("NU_PESSOA");

                entity.Property(e => e.CoGenero)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("CO_GENERO")
                    .IsFixedLength(true);

                entity.Property(e => e.DtNascimento)
                    .HasColumnType("date")
                    .HasColumnName("DT_NASCIMENTO");

                entity.Property(e => e.NoMae)
                    .HasMaxLength(70)
                    .IsUnicode(false)
                    .HasColumnName("NO_MAE");

                entity.Property(e => e.NoPessoa)
                    .IsRequired()
                    .HasMaxLength(70)
                    .IsUnicode(false)
                    .HasColumnName("NO_PESSOA");

                entity.Property(e => e.NoSocialPessoa)
                    .HasMaxLength(70)
                    .IsUnicode(false)
                    .HasColumnName("NO_SOCIAL_PESSOA");

                entity.Property(e => e.NuCpf)
                    .HasColumnType("numeric(11, 0)")
                    .HasColumnName("NU_CPF");
            });

            modelBuilder.Entity<Sdctb009PessoaStg>(entity =>
            {
                entity.HasKey(e => e.NuPessoa)
                    .HasName("PK_SDCTB009_STG");

                entity.ToTable("SDCTB009_PESSOA_STG");

                entity.HasIndex(e => e.NuCpf, "IX_SDCTB009_STG_1")
                    .IsUnique();

                entity.Property(e => e.NuPessoa).HasColumnName("NU_PESSOA");

                entity.Property(e => e.CoGenero)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("CO_GENERO")
                    .IsFixedLength(true);

                entity.Property(e => e.DtNascimento)
                    .HasColumnType("date")
                    .HasColumnName("DT_NASCIMENTO");

                entity.Property(e => e.NoMae)
                    .HasMaxLength(70)
                    .IsUnicode(false)
                    .HasColumnName("NO_MAE");

                entity.Property(e => e.NoPessoa)
                    .IsRequired()
                    .HasMaxLength(70)
                    .IsUnicode(false)
                    .HasColumnName("NO_PESSOA");

                entity.Property(e => e.NoSocialPessoa)
                    .HasMaxLength(70)
                    .IsUnicode(false)
                    .HasColumnName("NO_SOCIAL_PESSOA");

                entity.Property(e => e.NuCpf)
                    .HasColumnType("numeric(11, 0)")
                    .HasColumnName("NU_CPF");
            });

            modelBuilder.Entity<Sdctb010SituacaoPedido>(entity =>
            {
                entity.HasKey(e => e.NuSituacaoPedido)
                    .HasName("PK_SDCTB010");

                entity.ToTable("SDCTB010_SITUACAO_PEDIDO");

                entity.HasIndex(e => e.NuPedidoIndenizacao, "IX_SDCTB004_1");

                entity.Property(e => e.NuSituacaoPedido).HasColumnName("NU_SITUACAO_PEDIDO");

                entity.Property(e => e.CoMatriculaCaixa)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("CO_MATRICULA_CAIXA")
                    .IsFixedLength(true);

                entity.Property(e => e.DhExclusao)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_EXCLUSAO");

                entity.Property(e => e.DhSituacao)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_SITUACAO");

                entity.Property(e => e.NuMotivoIndeferimento).HasColumnName("NU_MOTIVO_INDEFERIMENTO");

                entity.Property(e => e.NuPedidoIndenizacao).HasColumnName("NU_PEDIDO_INDENIZACAO");

                entity.Property(e => e.NuTipoSituacaoPedido).HasColumnName("NU_TIPO_SITUACAO_PEDIDO");

                entity.HasOne(d => d.NuPedidoIndenizacaoNavigation)
                    .WithMany(p => p.Sdctb010SituacaoPedidos)
                    .HasForeignKey(d => d.NuPedidoIndenizacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB010_SDCTB008");

                entity.HasOne(d => d.NuTipoSituacaoPedidoNavigation)
                    .WithMany(p => p.Sdctb010SituacaoPedidos)
                    .HasForeignKey(d => d.NuTipoSituacaoPedido)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB010_SDCTB011");
            }); ;

            modelBuilder.Entity<Sdctb011TipoSituacaoPedido>(entity =>
            {
                entity.HasKey(e => e.NuTipoSituacaoPedido)
                    .HasName("PK_SDCTB011");

                entity.ToTable("SDCTB011_TIPO_SITUACAO_PEDIDO");

                entity.Property(e => e.NuTipoSituacaoPedido).HasColumnName("NU_TIPO_SITUACAO_PEDIDO");

                entity.Property(e => e.DeAbreviaturaSituacaoPedido)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("DE_ABREVIATURA_SITUACAO_PEDIDO");

                entity.Property(e => e.DeIcone)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("DE_ICONE");

                entity.Property(e => e.DeSituacaoPedido)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("DE_SITUACAO_PEDIDO");

                entity.Property(e => e.DhExclusao)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_EXCLUSAO");

                entity.Property(e => e.IcExibe).HasColumnName("IC_EXIBE");
            });

            modelBuilder.Entity<Sdctb012TipoVeiculo>(entity =>
            {
                entity.HasKey(e => e.NuTipoVeiculo)
                    .HasName("PK_SDCTB012");

                entity.ToTable("SDCTB012_TIPO_VEICULO");

                entity.Property(e => e.NuTipoVeiculo).HasColumnName("NU_TIPO_VEICULO");

                entity.Property(e => e.DeAbreviaturaTipoVeiculo)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("DE_ABREVIATURA_TIPO_VEICULO");

                entity.Property(e => e.DeTipoVeiculo)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("DE_TIPO_VEICULO");
            });

            modelBuilder.Entity<Sdctb013Uf>(entity =>
            {
                entity.HasKey(e => e.CoUf)
                    .HasName("PK_SDCTB013");

                entity.ToTable("SDCTB013_UF");

                entity.Property(e => e.CoUf)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("CO_UF")
                    .IsFixedLength(true);

                entity.Property(e => e.NoUf)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("NO_UF");
            });

            modelBuilder.Entity<Sdctb014TipoParticipacao>(entity =>
            {
                entity.HasKey(e => e.NuTipoParticipacao)
                    .HasName("PK_SDCTB014");

                entity.ToTable("SDCTB014_TIPO_PARTICIPACAO");

                entity.Property(e => e.NuTipoParticipacao).HasColumnName("NU_TIPO_PARTICIPACAO");

                entity.Property(e => e.DeAbreviaTipoParticipacao)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DE_ABREVIA_TIPO_PARTICIPACAO");

                entity.Property(e => e.DeTipoParticipacao)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("DE_TIPO_PARTICIPACAO");
            });

            modelBuilder.Entity<Sdctb015Participacao>(entity =>
            {
                entity.HasKey(e => e.NuParticipacao)
                     .HasName("PK_SDCTB015");

                entity.ToTable("SDCTB015_PARTICIPACAO");

                entity.HasIndex(e => new { e.NuPessoaParticipante, e.NuPedidoIndenizacao, e.NuTipoParticipacao }, "IX_SDCTB015_1");

                entity.HasIndex(e => e.NuMunicipio, "IX_SDCTB015_2");

                entity.Property(e => e.NuParticipacao).HasColumnName("NU_PARTICIPACAO");

                entity.Property(e => e.AaFabricacao).HasColumnName("AA_FABRICACAO");

                entity.Property(e => e.AaModelo).HasColumnName("AA_MODELO");

                entity.Property(e => e.CoDocumento)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("CO_DOCUMENTO");

                entity.Property(e => e.CoOrgaoEmissor)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("CO_ORGAO_EMISSOR");

                entity.Property(e => e.CoPosicaoImovel)
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasColumnName("CO_POSICAO_IMOVEL")
                    .IsFixedLength(true);

                entity.Property(e => e.CoUfOrgaoEmissor)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("CO_UF_ORGAO_EMISSOR")
                    .IsFixedLength(true);

                entity.Property(e => e.DeComplemento)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("DE_COMPLEMENTO");

                entity.Property(e => e.DeEmail)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("DE_EMAIL");

                entity.Property(e => e.DhFim)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_FIM");

                entity.Property(e => e.DtExpedicao)
                    .HasColumnType("date")
                    .HasColumnName("DT_EXPEDICAO");

                entity.Property(e => e.DtRenda)
                    .HasColumnType("date")
                    .HasColumnName("DT_RENDA");

                entity.Property(e => e.IcFormal).HasColumnName("IC_FORMAL");

                entity.Property(e => e.NoBairro)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("NO_BAIRRO");

                entity.Property(e => e.NoLogradouro)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("NO_LOGRADOURO");

                entity.Property(e => e.NuCep).HasColumnName("NU_CEP");

                entity.Property(e => e.NuDdd).HasColumnName("NU_DDD");

                entity.Property(e => e.NuEstadoCivil).HasColumnName("NU_ESTADO_CIVIL");

                entity.Property(e => e.NuMunicipio).HasColumnName("NU_MUNICIPIO");

                entity.Property(e => e.NuNacionalidade).HasColumnName("NU_NACIONALIDADE");

                entity.Property(e => e.NuNif)
                    .HasColumnType("decimal(11, 0)")
                    .HasColumnName("NU_NIF");

                entity.Property(e => e.NuOcupacao).HasColumnName("NU_OCUPACAO");

                entity.Property(e => e.NuParentesco).HasColumnName("NU_PARENTESCO");

                entity.Property(e => e.NuPedidoIndenizacao).HasColumnName("NU_PEDIDO_INDENIZACAO");

                entity.Property(e => e.NuPessoaParticipante).HasColumnName("NU_PESSOA_PARTICIPANTE");

                entity.Property(e => e.NuTelefone)
                    .HasColumnType("numeric(11, 0)")
                    .HasColumnName("NU_TELEFONE");

                entity.Property(e => e.NuTipoDocumento).HasColumnName("NU_TIPO_DOCUMENTO");

                entity.Property(e => e.NuTipoParticipacao).HasColumnName("NU_TIPO_PARTICIPACAO");

                entity.Property(e => e.NuTipoVeiculoProprietario).HasColumnName("NU_TIPO_VEICULO_PROPRIETARIO");

                entity.Property(e => e.SgTipoLogradouro)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("SG_TIPO_LOGRADOURO")
                    .IsFixedLength(true);

                entity.Property(e => e.VrPatrimonio)
                    .HasColumnType("money")
                    .HasColumnName("VR_PATRIMONIO");

                entity.Property(e => e.VrRenda)
                    .HasColumnType("money")
                    .HasColumnName("VR_RENDA");

                entity.HasOne(d => d.CoUfOrgaoEmissorNavigation)
                    .WithMany(p => p.Sdctb015Participacaos)
                    .HasForeignKey(d => d.CoUfOrgaoEmissor)
                    .HasConstraintName("FK_SDCTB015_SDCTB013");

                entity.HasOne(d => d.NuEstadoCivilNavigation)
                    .WithMany(p => p.Sdctb015Participacaos)
                    .HasForeignKey(d => d.NuEstadoCivil)
                    .HasConstraintName("FK_SDCTB015_SDCTB025");

                entity.HasOne(d => d.NuParentescoNavigation)
                    .WithMany(p => p.Sdctb015Participacaos)
                    .HasForeignKey(d => d.NuParentesco)
                    .HasConstraintName("FK_SDCTB015_SDCTB022");

                entity.HasOne(d => d.NuMunicipioNavigation)
                    .WithMany(p => p.Sdctb015Participacaos)
                    .HasForeignKey(d => d.NuMunicipio)
                    .HasConstraintName("FK_SDCTB015_SDCTB001");

                entity.HasOne(d => d.NuNacionalidadeNavigation)
                    .WithMany(p => p.Sdctb015Participacaos)
                    .HasForeignKey(d => d.NuNacionalidade)
                    .HasConstraintName("FK_SDCTB015_SDCTB020");

                entity.HasOne(d => d.NuOcupacaoNavigation)
                    .WithMany(p => p.Sdctb015Participacaos)
                    .HasForeignKey(d => d.NuOcupacao)
                    .HasConstraintName("FK_SDCTB015_SDCTB021");

                entity.HasOne(d => d.NuPedidoIndenizacaoNavigation)
                    .WithMany(p => p.Sdctb015Participacaos)
                    .HasForeignKey(d => d.NuPedidoIndenizacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB015_SDCTB008");

                entity.HasOne(d => d.NuPessoaParticipanteNavigation)
                    .WithMany(p => p.Sdctb015Participacaos)
                    .HasForeignKey(d => d.NuPessoaParticipante)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB015_SDCTB009");

                entity.HasOne(d => d.NuTipoDocumentoNavigation)
                    .WithMany(p => p.Sdctb015Participacaos)
                    .HasForeignKey(d => d.NuTipoDocumento)
                    .HasConstraintName("FK_SDCTB015_SDCTB006");

                entity.HasOne(d => d.NuTipoParticipacaoNavigation)
                    .WithMany(p => p.Sdctb015Participacaos)
                    .HasForeignKey(d => d.NuTipoParticipacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB015_SDCTB014");

                entity.HasOne(d => d.NuTipoVeiculoProprietarioNavigation)
                    .WithMany(p => p.Sdctb015Participacaos)
                    .HasForeignKey(d => d.NuTipoVeiculoProprietario)
                    .HasConstraintName("FK_SDCTB015_SDCTB012");

                entity.HasOne(d => d.SgTipoLogradouroNavigation)
                    .WithMany(p => p.Sdctb015Participacaos)
                    .HasForeignKey(d => d.SgTipoLogradouro)
                    .HasConstraintName("FK_SDCTB015_SDCTB004");
            });

            modelBuilder.Entity<Sdctb016Canal>(entity =>
            {
                entity.HasKey(e => e.NuCanal)
                    .HasName("PK_SDCTB016");

                entity.ToTable("SDCTB016_CANAL");

                entity.Property(e => e.NuCanal).HasColumnName("NU_CANAL");

                entity.Property(e => e.DeCanal)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("DE_CANAL");
            });

            modelBuilder.Entity<Sdctb017TipoMotivoSituacao>(entity =>
            {
                entity.HasKey(e => e.NuTipoMotivoSituacao)
                    .HasName("PK_SDCTB017");

                entity.ToTable("SDCTB017_TIPO_MOTIVO_SITUACAO");

                entity.Property(e => e.NuTipoMotivoSituacao).HasColumnName("NU_TIPO_MOTIVO_SITUACAO");

                entity.Property(e => e.DeAbreviaturaMotivoSituacao)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("DE_ABREVIATURA_MOTIVO_SITUACAO");

                entity.Property(e => e.DeTipoMotivoSituacao)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("DE_TIPO_MOTIVO_SITUACAO");

                entity.Property(e => e.DhExclusao)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_EXCLUSAO");

                entity.Property(e => e.IcExibe).HasColumnName("IC_EXIBE");
            });

            modelBuilder.Entity<Sdctb018SituacaoMotivo>(entity =>
            {
                entity.HasKey(e => e.NuSituacaoMotivo)
                    .HasName("PK_SDCTB018");

                entity.ToTable("SDCTB018_SITUACAO_MOTIVO");

                entity.HasIndex(e => e.NuSituacaoPedido, "IX_SDCTB018_1");

                entity.Property(e => e.NuSituacaoMotivo).HasColumnName("NU_SITUACAO_MOTIVO");

                entity.Property(e => e.DhExclusao)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_EXCLUSAO");

                entity.Property(e => e.DhSituacao)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_SITUACAO");

                entity.Property(e => e.NuSituacaoPedido).HasColumnName("NU_SITUACAO_PEDIDO");

                entity.Property(e => e.NuTipoMotivoSituacao).HasColumnName("NU_TIPO_MOTIVO_SITUACAO");

                entity.HasOne(d => d.NuSituacaoPedidoNavigation)
                    .WithMany(p => p.Sdctb018SituacaoMotivos)
                    .HasForeignKey(d => d.NuSituacaoPedido)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB018_SDCTB010");

                entity.HasOne(d => d.NuTipoMotivoSituacaoNavigation)
                    .WithMany(p => p.Sdctb018SituacaoMotivos)
                    .HasForeignKey(d => d.NuTipoMotivoSituacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB018_SDCTB017");
            });

            modelBuilder.Entity<Sdctb019ValorIndenizacao>(entity =>
            {
                entity.HasKey(e => e.NuValorIndenizacao)
                    .HasName("PK_SDCTB019");

                entity.ToTable("SDCTB019_VALOR_INDENIZACAO");

                entity.HasIndex(e => new { e.NuPedidoIndenizacao, e.NuTipoIndenizacaoPgto }, "IX_SDCTB019_1")
                    .IsUnique();

                entity.Property(e => e.NuValorIndenizacao).HasColumnName("NU_VALOR_INDENIZACAO");

                entity.Property(e => e.DhExclusao)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_EXCLUSAO");

                entity.Property(e => e.DtEfetivaCredito)
                    .HasColumnType("date")
                    .HasColumnName("DT_EFETIVA_CREDITO");

                entity.Property(e => e.DtPrevistaCredito)
                    .HasColumnType("date")
                    .HasColumnName("DT_PREVISTA_CREDITO");

                entity.Property(e => e.NuPedidoIndenizacao).HasColumnName("NU_PEDIDO_INDENIZACAO");

                entity.Property(e => e.NuTipoIndenizacaoPgto).HasColumnName("NU_TIPO_INDENIZACAO_PGTO");

                entity.Property(e => e.VrIndenizacao)
                    .HasColumnType("money")
                    .HasColumnName("VR_INDENIZACAO");

                entity.Property(e => e.DeObservacao)                    
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("DE_OBSERVACAO");

                entity.HasOne(d => d.NuPedidoIndenizacaoNavigation)
                    .WithMany(p => p.Sdctb019ValorIndenizacaos)
                    .HasForeignKey(d => d.NuPedidoIndenizacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB019_SDCTB008");

                entity.HasOne(d => d.NuTipoIndenizacaoPgtoNavigation)
                    .WithMany(p => p.Sdctb019ValorIndenizacaos)
                    .HasForeignKey(d => d.NuTipoIndenizacaoPgto)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB019_SDCTB007");
            });

            modelBuilder.Entity<Sdctb020Nacionalidade>(entity =>
            {
                entity.HasKey(e => e.NuNacionalidade);

                entity.ToTable("SDCTB020_NACIONALIDADE");

                entity.Property(e => e.NuNacionalidade).HasColumnName("NU_NACIONALIDADE");

                entity.Property(e => e.NoNacionalidade)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("NO_NACIONALIDADE");

                entity.Property(e => e.NuChaveIco).HasColumnName("NU_CHAVE_ICO");
            });

            modelBuilder.Entity<Sdctb021Ocupacao>(entity =>
            {
                entity.HasKey(e => e.NuOcupacao);

                entity.ToTable("SDCTB021_OCUPACAO");

                entity.Property(e => e.NuOcupacao).HasColumnName("NU_OCUPACAO");

                entity.Property(e => e.DhExclusao)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_EXCLUSAO");

                entity.Property(e => e.NoOcupacao)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("NO_OCUPACAO");

                entity.Property(e => e.NuChaveCli).HasColumnName("NU_CHAVE_CLI");

                entity.Property(e => e.TpOcupacao)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("TP_OCUPACAO")
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<Sdctb022Parentesco>(entity =>
            {
                entity.HasKey(e => e.NuParentesco)
                    .HasName("PK_SDCTB022");

                entity.ToTable("SDCTB022_PARENTESCO");

                entity.Property(e => e.NuParentesco).HasColumnName("NU_PARENTESCO");

                entity.Property(e => e.NoParentesco)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("NO_PARENTESCO");
            });

            modelBuilder.Entity<Sdctb023DeclaraHerdeiro>(entity =>
            {
                entity.HasKey(e => e.NuDeclaracaoHerdeiro);

                entity.ToTable("SDCTB023_DECLARA_HERDEIRO");

                entity.HasIndex(e => e.NuPedidoIndenizacao, "IX_SDCTB023_1");

                entity.HasIndex(e => e.NuPessoaHerdeiro, "IX_SDCTB023_2");

                entity.Property(e => e.NuDeclaracaoHerdeiro).HasColumnName("NU_DECLARACAO_HERDEIRO");

                entity.Property(e => e.DhExclusao)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_EXCLUSAO");

                entity.Property(e => e.NuParentesco).HasColumnName("NU_PARENTESCO");

                entity.Property(e => e.NuPedidoIndenizacao).HasColumnName("NU_PEDIDO_INDENIZACAO");

                entity.Property(e => e.NuPessoaHerdeiro).HasColumnName("NU_PESSOA_HERDEIRO");

                entity.HasOne(d => d.NuParentescoNavigation)
                    .WithMany(p => p.Sdctb023DeclaraHerdeiros)
                    .HasForeignKey(d => d.NuParentesco)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB023_SDCTB022");

                entity.HasOne(d => d.NuPedidoIndenizacaoNavigation)
                    .WithMany(p => p.Sdctb023DeclaraHerdeiros)
                    .HasForeignKey(d => d.NuPedidoIndenizacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB023_SDCTB008");

                entity.HasOne(d => d.NuPessoaHerdeiroNavigation)
                    .WithMany(p => p.Sdctb023DeclaraHerdeiros)
                    .HasForeignKey(d => d.NuPessoaHerdeiro)
                    .HasConstraintName("FK_SDCTB023_SDCTB009");
            });

            modelBuilder.Entity<Sdctb024SolicitanteIndenizacao>(entity =>
            {
                entity.HasKey(e => e.NuSolicitanteIndenizacao)
                    .HasName("PK_SDCTB024");

                entity.ToTable("SDCTB024_SOLICITANTE_INDENIZACAO");

                entity.Property(e => e.NuSolicitanteIndenizacao).HasColumnName("NU_SOLICITANTE_INDENIZACAO");

                entity.Property(e => e.DhExclusao)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_EXCLUSAO");

                entity.Property(e => e.NuTipoIndenizacao).HasColumnName("NU_TIPO_INDENIZACAO");

                entity.Property(e => e.NuTipoParticipacao).HasColumnName("NU_TIPO_PARTICIPACAO");

                entity.HasOne(d => d.NuTipoIndenizacaoNavigation)
                    .WithMany(p => p.Sdctb024SolicitanteIndenizacaos)
                    .HasForeignKey(d => d.NuTipoIndenizacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB024_SDCTB007");

                entity.HasOne(d => d.NuTipoParticipacaoNavigation)
                    .WithMany(p => p.Sdctb024SolicitanteIndenizacaos)
                    .HasForeignKey(d => d.NuTipoParticipacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB024_SDCTB014");
            });

            modelBuilder.Entity<Sdctb025EstadoCivil>(entity =>
            {
                entity.HasKey(e => e.NuEstadoCivil);

                entity.ToTable("SDCTB025_ESTADO_CIVIL");

                entity.Property(e => e.NuEstadoCivil)
                    .ValueGeneratedNever()
                    .HasColumnName("NU_ESTADO_CIVIL");

                entity.Property(e => e.NoEstadoCivil)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("NO_ESTADO_CIVIL");
            });

            modelBuilder.Entity<Sdctb026DeclaraHerdeiroComplemento>(entity =>
            {
                entity.HasKey(e => e.NuHerdeiroComplemento)
                    .HasName("PK_SDCTB026");

                entity.ToTable("SDCTB026_DECLARA_HERDEIRO_COMPLEMENTO");

                entity.HasIndex(e => e.NuPedidoIndenizacao, "IX_SDCTB026_1");

                entity.Property(e => e.NuHerdeiroComplemento).HasColumnName("NU_HERDEIRO_COMPLEMENTO");

                entity.Property(e => e.DhExclusao)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_EXCLUSAO");

                entity.Property(e => e.IcPossuiNascituro).HasColumnName("IC_POSSUI_NASCITURO");

                entity.Property(e => e.NuPedidoIndenizacao).HasColumnName("NU_PEDIDO_INDENIZACAO");

                entity.Property(e => e.QtFilhosFalecidos).HasColumnName("QT_FILHOS_FALECIDOS");

                entity.Property(e => e.QtIrmaosFalecidos).HasColumnName("QT_IRMAOS_FALECIDOS");

                entity.HasOne(d => d.NuPedidoIndenizacaoNavigation)
                    .WithMany(p => p.Sdctb026DeclaraHerdeiroComplementos)
                    .HasForeignKey(d => d.NuPedidoIndenizacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB026_SCDTB008");
            });

            modelBuilder.Entity<Sdctb027Notificacao>(entity =>
            {
                entity.HasKey(e => e.NuNotificacao);

                entity.ToTable("SDCTB027_NOTIFICACAO");

                entity.HasIndex(e => e.NuSituacaoMotivo, "IX_SDCTB027_1");

                entity.HasIndex(e => e.NuPessoaNotificada, "IX_SDCTB027_2");

                entity.Property(e => e.NuNotificacao).HasColumnName("NU_NOTIFICACAO");

                entity.Property(e => e.DeMensagem)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("DE_MENSAGEM");

                entity.Property(e => e.DhCienciaNotificacao)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_CIENCIA_NOTIFICACAO");

                entity.Property(e => e.DhEnvioNotificacao)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_ENVIO_NOTIFICACAO");

                entity.Property(e => e.NuPessoaNotificada).HasColumnName("NU_PESSOA_NOTIFICADA");

                entity.Property(e => e.NuSituacaoMotivo).HasColumnName("NU_SITUACAO_MOTIVO");

                entity.HasOne(d => d.NuPessoaNotificadaNavigation)
                    .WithMany(p => p.Sdctb027Notificacaos)
                    .HasForeignKey(d => d.NuPessoaNotificada)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB027_SDCTB009");

                entity.HasOne(d => d.NuSituacaoMotivoNavigation)
                    .WithMany(p => p.Sdctb027Notificacaos)
                    .HasForeignKey(d => d.NuSituacaoMotivo)
                    .HasConstraintName("FK_SDCTB027_SDCTB018");
            });

            modelBuilder.Entity<Sdctb028TipoAcordo>(entity =>
            {
                entity.HasKey(e => e.NuTipoAcordo);

                entity.ToTable("SDCTB028_TIPO_ACORDO");

                entity.Property(e => e.NuTipoAcordo).HasColumnName("NU_TIPO_ACORDO");

                entity.Property(e => e.DeAcordo)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("DE_ACORDO");

                entity.Property(e => e.DeAcordoAbreviatura)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("DE_ACORDO_ABREVIATURA");

                entity.Property(e => e.DhExclusao)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_EXCLUSAO");
            });

            modelBuilder.Entity<Sdctb029Acordo>(entity =>
            {
                entity.HasKey(e => e.NuAcordo);

                entity.ToTable("SDCTB029_ACORDO");

                entity.HasIndex(e => e.NuPedidoIndenizacao, "IX_SDCTB029_1");

                entity.Property(e => e.NuAcordo).HasColumnName("NU_ACORDO");

                entity.Property(e => e.DhFim)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_FIM");

                entity.Property(e => e.DhInicio)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_INICIO");

                entity.Property(e => e.NuPedidoIndenizacao).HasColumnName("NU_PEDIDO_INDENIZACAO");

                entity.Property(e => e.NuTipoAcordo).HasColumnName("NU_TIPO_ACORDO");

                entity.HasOne(d => d.NuPedidoIndenizacaoNavigation)
                    .WithMany(p => p.Sdctb029Acordos)
                    .HasForeignKey(d => d.NuPedidoIndenizacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB029_SDCTB008");

                entity.HasOne(d => d.NuTipoAcordoNavigation)
                    .WithMany(p => p.Sdctb029Acordos)
                    .HasForeignKey(d => d.NuTipoAcordo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB029_SDCTB028");
            });

            modelBuilder.Entity<Sdctb030DocumentoPendente>(entity =>
            {
                entity.HasKey(e => e.NuDocumentoPendente);

                entity.ToTable("SDCTB030_DOCUMENTO_PENDENTE");

                entity.HasIndex(e => e.NuSituacaoPedido, "IX_SDCTB030_1");

                entity.Property(e => e.NuDocumentoPendente).HasColumnName("NU_DOCUMENTO_PENDENTE");

                entity.Property(e => e.DhExclusao)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_EXCLUSAO");

                entity.Property(e => e.NuDocumentoExigido).HasColumnName("NU_DOCUMENTO_EXIGIDO");

                entity.Property(e => e.NuSituacaoPedido).HasColumnName("NU_SITUACAO_PEDIDO");

                entity.HasOne(d => d.NuDocumentoExigidoNavigation)
                    .WithMany(p => p.Sdctb030DocumentoPendentes)
                    .HasForeignKey(d => d.NuDocumentoExigido)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB030_SDCTB003");

                entity.HasOne(d => d.NuSituacaoPedidoNavigation)
                    .WithMany(p => p.Sdctb030DocumentoPendentes)
                    .HasForeignKey(d => d.NuSituacaoPedido)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB030_SDCTB010");

            });

            modelBuilder.Entity<Sdctb034Emitente>(entity =>
            {
                entity.HasKey(e => e.NuEmitente)
                    .HasName("PK_SDCTB034");

                entity.ToTable("SDCTB034_EMITENTE");

                entity.HasIndex(e => e.NuDocumentoCapturado, "IX_SDCTB034_1");

                entity.Property(e => e.NuEmitente).HasColumnName("NU_EMITENTE");

                entity.Property(e => e.NuDocumentoCapturado).HasColumnName("NU_DOCUMENTO_CAPTURADO");

                entity.Property(e => e.NuIdentificacao).HasColumnName("NU_IDENTIFICACAO");

                entity.Property(e => e.NuRecibo)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("NU_RECIBO");

                entity.HasOne(d => d.NuDocumentoCapturadoNavigation)
                    .WithMany(p => p.Sdctb034Emitentes)
                    .HasForeignKey(d => d.NuDocumentoCapturado)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB034_SDCTB002");
            });

            modelBuilder.Entity<Sdctb035ItemDespesa>(entity =>
            {
                entity.HasKey(e => e.NuItemDespesa)
                    .HasName("PK_SDCTB035");

                entity.ToTable("SDCTB035_ITEM_DESPESA");

                entity.HasIndex(e => e.NuEmitente, "IX_SDCTB035_1");

                entity.Property(e => e.NuItemDespesa).HasColumnName("NU_ITEM_DESPESA");

                entity.Property(e => e.NuEmitente).HasColumnName("NU_EMITENTE");

                entity.Property(e => e.NuTipoItemDespesa).HasColumnName("NU_TIPO_ITEM_DESPESA");

                entity.Property(e => e.VrItemDespesa)
                    .HasColumnType("money")
                    .HasColumnName("VR_ITEM_DESPESA");

                entity.HasOne(d => d.NuEmitenteNavigation)
                    .WithMany(p => p.Sdctb035ItemDespesas)
                    .HasForeignKey(d => d.NuEmitente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB035_SDCTB034");

                entity.HasOne(d => d.NuTipoItemDespesaNavigation)
                    .WithMany(p => p.Sdctb035ItemDespesas)
                    .HasForeignKey(d => d.NuTipoItemDespesa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB035_SDCTB036");
            });

            modelBuilder.Entity<Sdctb036TipoItemDespesa>(entity =>
            {
                entity.HasKey(e => e.NuTipoItemDespesa)
                    .HasName("PK_SDCTB036");

                entity.ToTable("SDCTB036_TIPO_ITEM_DESPESA");

                entity.Property(e => e.NuTipoItemDespesa)
                    .ValueGeneratedNever()
                    .HasColumnName("NU_TIPO_ITEM_DESPESA");

                entity.Property(e => e.DeTipoItemDespesa)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("DE_TIPO_ITEM_DESPESA");
            });

            modelBuilder.Entity<Sdctb044TipoIndeferimento>(entity =>
            {
                entity.HasKey(e => e.NuTipoIndeferimento)
                    .HasName("PK_SDCTB044");

                entity.ToTable("SDCTB044_TIPO_INDEFERIMENTO");

                entity.Property(e => e.NuTipoIndeferimento)
                    .ValueGeneratedNever()
                    .HasColumnName("NU_TIPO_INDEFERIMENTO");

                entity.Property(e => e.DeTipoIndeferimento)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("DE_TIPO_INDEFERIMENTO");
            });

            modelBuilder.Entity<Sdctb045MotivoIndeferimento>(entity =>
            {
                entity.HasKey(e => e.NuMotivoIndeferimento)
                    .HasName("PK_SDCTB045");

                entity.ToTable("SDCTB045_MOTIVO_INDEFERIMENTO");

                entity.Property(e => e.NuMotivoIndeferimento)
                    .ValueGeneratedNever()
                    .HasColumnName("NU_MOTIVO_INDEFERIMENTO");

                entity.Property(e => e.DeMotivoIndeferimento)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("DE_MOTIVO_INDEFERIMENTO");

                entity.Property(e => e.NuTipoIndeferimento).HasColumnName("NU_TIPO_INDEFERIMENTO");

                entity.HasOne(d => d.NuTipoIndeferimentoNavigation)
                    .WithMany(p => p.Sdctb045MotivoIndeferimentos)
                    .HasForeignKey(d => d.NuTipoIndeferimento)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB045_SDCTB044");
            });

            modelBuilder.Entity<Sdctb046VigenciaIndenizacao>(entity =>
            {
                entity.HasKey(e => e.NuVigenciaIndenizacao)
                    .HasName("PK_SDCTB046");

                entity.ToTable("SDCTB046_VIGENCIA_INDENIZACAO");

                entity.Property(e => e.NuVigenciaIndenizacao).HasColumnName("NU_VIGENCIA_INDENIZACAO");

                entity.Property(e => e.DtFimVigencia)
                    .HasColumnType("date")
                    .HasColumnName("DT_FIM_VIGENCIA");

                entity.Property(e => e.DtInicioVigencia)
                    .HasColumnType("date")
                    .HasColumnName("DT_INICIO_VIGENCIA");

                entity.Property(e => e.NuTipoIndenizacao).HasColumnName("NU_TIPO_INDENIZACAO");

                entity.Property(e => e.VrLimiteIndenizacao)
                    .HasColumnType("money")
                    .HasColumnName("VR_LIMITE_INDENIZACAO");

                entity.HasOne(d => d.NuTipoIndenizacaoNavigation)
                    .WithMany(p => p.Sdctb046VigenciaIndenizacaos)
                    .HasForeignKey(d => d.NuTipoIndenizacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB046_SDCTB007");
            });

            modelBuilder.Entity<Sdctb047TipoItemDocumento>(entity =>
            {
                entity.HasKey(e => e.NuItemDocumento)
                    .HasName("PK_SDCTB047");

                entity.ToTable("SDCTB047_TIPO_ITEM_DOCUMENTO");

                entity.Property(e => e.NuItemDocumento)
                    .ValueGeneratedNever()
                    .HasColumnName("NU_ITEM_DOCUMENTO");

                entity.Property(e => e.DeItemDocumento)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("DE_ITEM_DOCUMENTO");

                entity.Property(e => e.DeItemDocumentoTitulo)
                    .IsRequired()
                    .HasMaxLength(70)
                    .IsUnicode(false)
                    .HasColumnName("DE_ITEM_DOCUMENTO_TITULO");

                entity.Property(e => e.IcNumerico).HasColumnName("IC_NUMERICO");

                entity.Property(e => e.NuOrdem).HasColumnName("NU_ORDEM");

                entity.Property(e => e.NuTamanho).HasColumnName("NU_TAMANHO");
            });

            modelBuilder.Entity<Sdctb048ItemDocumentoExigido>(entity =>
            {
                entity.HasKey(e => e.NuItemDocumentoExigido)
                    .HasName("PK_SDCTB048");

                entity.ToTable("SDCTB048_ITEM_DOCUMENTO_EXIGIDO");

                entity.HasIndex(e => new { e.NuTipoDocumento, e.NuItemDocumento }, "IX_SDCTB048_1")
                    .IsUnique();

                entity.Property(e => e.NuItemDocumentoExigido).HasColumnName("NU_ITEM_DOCUMENTO_EXIGIDO");

                entity.Property(e => e.NuItemDocumento).HasColumnName("NU_ITEM_DOCUMENTO");

                entity.Property(e => e.NuTipoDocumento).HasColumnName("NU_TIPO_DOCUMENTO");

                entity.HasOne(d => d.NuItemDocumentoNavigation)
                    .WithMany(p => p.Sdctb048ItemDocumentoExigidos)
                    .HasForeignKey(d => d.NuItemDocumento)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB048_SDCTB047");
            });

            modelBuilder.Entity<Sdctb049ItemDocumentoConteudo>(entity =>
            {
                entity.HasKey(e => e.NuItemDocumentoConteudo)
                    .HasName("PK_SDCTB049");

                entity.ToTable("SDCTB049_ITEM_DOCUMENTO_CONTEUDO");

                entity.HasIndex(e => new { e.NuDocumentoCapturado, e.NuItemDocumento }, "IX_SDCTB049_1")
                    .IsUnique();

                entity.Property(e => e.NuItemDocumentoConteudo).HasColumnName("NU_ITEM_DOCUMENTO_CONTEUDO");

                entity.Property(e => e.DeConteudo)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("DE_CONTEUDO");

                entity.Property(e => e.NuDocumentoCapturado).HasColumnName("NU_DOCUMENTO_CAPTURADO");

                entity.Property(e => e.NuItemDocumento).HasColumnName("NU_ITEM_DOCUMENTO");

                entity.HasOne(d => d.NuDocumentoCapturadoNavigation)
                    .WithMany(p => p.Sdctb049ItemDocumentoConteudos)
                    .HasForeignKey(d => d.NuDocumentoCapturado)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB049_SDCTB002");

                entity.HasOne(d => d.NuItemDocumentoNavigation)
                    .WithMany(p => p.Sdctb049ItemDocumentoConteudos)
                    .HasForeignKey(d => d.NuItemDocumento)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB049_SDCTB047");
            });

            modelBuilder.Entity<Sdctb054GedamPedidoIndenizacao>(entity =>
            {
                entity.HasKey(e => e.NuGedamPedidoIndenizacao)
                    .HasName("PK_SDCTB054");

                entity.ToTable("SDCTB054_GEDAM_PEDIDO_INDENIZACAO");

                entity.HasIndex(e => new { e.NuGedamPedido, e.NuPedidoIndenizacao }, "IX_SDCTB054_1")
                    .IsUnique();

                entity.HasIndex(e => new { e.NuCpf, e.DhPedido }, "IX_SDCTB054_2")
                    .IsUnique();

                entity.Property(e => e.NuGedamPedidoIndenizacao).HasColumnName("NU_GEDAM_PEDIDO_INDENIZACAO");

                entity.Property(e => e.DhPedido)
                    .HasColumnType("datetime")
                    .HasColumnName("DH_PEDIDO");

                entity.Property(e => e.NuCpf).HasColumnName("NU_CPF");

                entity.Property(e => e.NuGedamPedido).HasColumnName("NU_GEDAM_PEDIDO");

                entity.Property(e => e.NuPedidoIndenizacao).HasColumnName("NU_PEDIDO_INDENIZACAO");


                entity.HasOne(d => d.NuPedidoIndenizacaoNavigation)
                    .WithMany(p => p.Sdctb054GedamPedidoIndenizacaos)
                    .HasForeignKey(d => d.NuPedidoIndenizacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SDCTB054_SDCTB008");
            });    
        }
    }

    public class DbEscrita : DbContextBase<DbEscrita>
    {
        public DbEscrita(DbContextOptions<DbEscrita> options) : base(options) { }
    }

    public class DbLeitura : DbContextBase<DbLeitura>
    {
        public DbLeitura(DbContextOptions<DbLeitura> options) : base(options) { }

    }

    public class DbLeituraHistorico : DbContext
    {
        public DbLeituraHistorico(DbContextOptions<DbLeituraHistorico> options) : base(options) { }

        public virtual DbSet<Dpvtb013PrepSinistro> Dpvtb013PrepSinistro { get; set; }
        public virtual DbSet<Dpvtb014PrepPessoa> Dpvtb014PrepPessoa { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //configurar aqui os relacionamentos das tabelas
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AI");

            modelBuilder.Entity<Dpvtb013PrepSinistro>(entity =>
            {

                entity.HasKey(e => e.IdSinistro);

                entity.ToTable("DPVTB013_PREP_SINISTRO");

                entity.Property(e => e.IdSinistro)
                    .HasColumnType("int")
                    .HasColumnName("ID_SINISTRO");

                entity.Property(e => e.CdAvisoSinistro)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("CD_AVISO_SINISTRO");

                entity.Property(e => e.DtCriacaoAvisoSinistro)
                    .HasColumnType("datetime")
                    .HasColumnName("DT_CRIACAO_AVISO_SINISTRO");

                entity.Property(e => e.DtCriacaoNumeroSinistro)
                    .HasColumnType("datetime")
                    .HasColumnName("DT_CRIACAO_NUMERO_SINISTRO");

                entity.Property(e => e.NuSinistro)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("NU_SINISTRO");

                entity.Property(e => e.NuSequenciaSinistro)
                .HasColumnType("int")
                .HasColumnName("NU_SEQUENCIA_SINISTRO");

                entity.Property(e => e.DsNaturezaSinistro)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("DS_NATUREZA_SINISTRO");

                entity.Property(e => e.DsTipoInvalidez)
                   .HasMaxLength(10)
                   .IsUnicode(false)
                   .HasColumnName("DS_TIPO_INVALIDEZ");

                entity.Property(e => e.SgUfOcorrenciaSinistro)
                   .HasMaxLength(2)
                   .IsUnicode(false)
                   .HasColumnName("SG_UF_OCORRENCIA_SINISTRO");

                entity.Property(e => e.NmMuniOcorrenciaSinistro)
                  .HasMaxLength(100)
                  .IsUnicode(false)
                  .HasColumnName("NM_MUNI_OCORRENCIA_SINISTRO");

                entity.Property(e => e.DsDelegaciaOcorrencia)
                  .HasMaxLength(100)
                  .IsUnicode(false)
                  .HasColumnName("DS_DELEGACIA_OCORRENCIA");

                entity.Property(e => e.DtOcorrenciaSinistro)
                   .HasColumnType("datetime")
                   .HasColumnName("DT_OCORRENCIA_SINISTRO");

                entity.Property(e => e.DtReclamacaoSinistro)
                   .HasColumnType("datetime")
                   .HasColumnName("DT_RECLAMACAO_SINISTRO");

                entity.Property(e => e.SgUfDelegaciaOcorrencia)
                  .HasMaxLength(2)
                  .IsUnicode(false)
                  .HasColumnName("SG_UF_DELEGACIA_OCORRENCIA");

                entity.Property(e => e.NuBoletimOcorrencia)
                 .HasMaxLength(200)
                 .IsUnicode(false)
                 .HasColumnName("NU_BOLETIM_OCORRENCIA");

                entity.Property(e => e.DtBoletimOcorrencia)
                   .HasColumnType("datetime")
                   .HasColumnName("DT_BOLETIM_OCORRENCIA");

                entity.Property(e => e.NuLaudoIml)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("NU_LAUDO_IML");

                entity.Property(e => e.SgUfObitoVitima)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("SG_UF_OBITO_VITIMA");

                entity.Property(e => e.DtObitoVitima)
                   .HasColumnType("datetime")
                   .HasColumnName("DT_OBITO_VITIMA");

                entity.Property(e => e.VlPleiteado)
                  .HasColumnType("decimal")
                  .HasColumnName("VL_PLEITEADO");

                entity.Property(e => e.NmMedicoPrimeiroAtendimento)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NM_MEDICO_PRIMEIRO_ATENDIMENTO");

                entity.Property(e => e.NuCrmMedicoPrimAtend)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NU_CRM_MEDICO_PRIM_ATEND");

                entity.Property(e => e.SgUfCrmMedicoPrimAtend)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("SG_UF_CRM_MEDICO_PRIM_ATEND");

                entity.Property(e => e.DsRazaoNegativa)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DS_RAZAO_NEGATIVA");

                entity.Property(e => e.DsMotivoNegativa)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DS_MOTIVO_NEGATIVA");

                entity.Property(e => e.DsMotivoBloqueio)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DS_MOTIVO_BLOQUEIO");

                entity.Property(e => e.NmVitima)
               .HasMaxLength(100)
               .IsUnicode(false)
               .HasColumnName("NM_VITIMA");

                entity.Property(e => e.DtNascimentoVitima)
                   .HasColumnType("datetime")
                   .HasColumnName("DT_NASCIMENTO_VITIMA");

                entity.Property(e => e.NuCpfVitima)
                .HasColumnType("bigint")
                .HasColumnName("NU_CPF_VITIMA");

                entity.Property(e => e.NmTitularCpf)
               .HasMaxLength(100)
               .IsUnicode(false)
               .HasColumnName("NM_TITULAR_CPF");

                entity.Property(e => e.NmMaeTitularCpf)
               .HasMaxLength(100)
               .IsUnicode(false)
               .HasColumnName("NM_MAE_TITULAR_CPF");

                entity.Property(e => e.NmUltimoPortador)
               .HasMaxLength(100)
               .IsUnicode(false)
               .HasColumnName("NM_ULTIMO_PORTADOR");

                entity.Property(e => e.NuCpfUltimoPortador)
                .HasColumnType("bigint")
                .HasColumnName("NU_CPF_ULTIMO_PORTADOR");

                entity.Property(e => e.DsTipoCpfVitima)
               .HasMaxLength(20)
               .IsUnicode(false)
               .HasColumnName("DS_TIPO_CPF_VITIMA");

                entity.Property(e => e.DsEmailVitima)
               .HasMaxLength(120)
               .IsUnicode(false)
               .HasColumnName("DS_EMAIL_VITIMA");

                entity.Property(e => e.SgUfVitima)
               .HasMaxLength(2)
               .IsUnicode(false)
               .HasColumnName("SG_UF_VITIMA");

                entity.Property(e => e.DsSexoVitima)
               .HasMaxLength(10)
               .IsUnicode(false)
               .HasColumnName("DS_SEXO_VITIMA");

                entity.Property(e => e.DsTipoSinistrado)
               .HasMaxLength(20)
               .IsUnicode(false)
               .HasColumnName("DS_TIPO_SINISTRADO");

                entity.Property(e => e.DsTipoDocVeiculo)
               .HasMaxLength(200)
               .IsUnicode(false)
               .HasColumnName("DS_TIPO_DOC_VEICULO");

                entity.Property(e => e.SgUfPlacaVeiculo)
              .HasMaxLength(2)
              .IsUnicode(false)
              .HasColumnName("SG_UF_PLACA_VEICULO");

                entity.Property(e => e.NuPlacaVeiculo)
               .HasMaxLength(100)
               .IsUnicode(false)
               .HasColumnName("NU_PLACA_VEICULO");

                entity.Property(e => e.CdCategoriaVeiculo)
               .HasMaxLength(2)
               .IsUnicode(false)
               .HasColumnName("CD_CATEGORIA_VEICULO");

                entity.Property(e => e.DsCategoriaVeiculo)
              .HasMaxLength(200)
              .IsUnicode(false)
              .HasColumnName("DS_CATEGORIA_VEICULO");

                entity.Property(e => e.NmEmpresaDigitalizadora)
              .HasMaxLength(100)
              .IsUnicode(false)
              .HasColumnName("NM_EMPRESA_DIGITALIZADORA");

                entity.Property(e => e.NmSituacaoAtualSinistro)
              .HasMaxLength(100)
              .IsUnicode(false)
              .HasColumnName("NM_SITUACAO_ATUAL_SINISTRO");

                entity.Property(e => e.DtUltimoMovimentoSinistro)
                   .HasColumnType("datetime")
                   .HasColumnName("DT_ULTIMO_MOVIMENTO_SINISTRO");

                entity.Property(e => e.DtUltimoDocumento)
                   .HasColumnType("datetime")
                   .HasColumnName("DT_ULTIMO_DOCUMENTO");

                entity.Property(e => e.NmAcaoSinistro)
              .HasMaxLength(60)
              .IsUnicode(false)
              .HasColumnName("NM_ACAO_SINISTRO");

                entity.Property(e => e.NmSituacaoPosterior)
             .HasMaxLength(60)
             .IsUnicode(false)
             .HasColumnName("NM_SITUACAO_POSTERIOR");

                entity.Property(e => e.DtInclusaoDado)
                   .HasColumnType("datetime")
                   .HasColumnName("DT_INCLUSAO_DADO");
            });

            modelBuilder.Entity<Dpvtb014PrepPessoa>(entity =>
            {
                entity.HasKey(e => e.IdSinistro);

                entity.ToTable("DPVTB014_PREP_PESSOA");

                entity.Property(e => e.IdSinistro)
                    .ValueGeneratedNever()
                    .HasColumnName("ID_SINISTRO");

                entity.Property(e => e.NuSinistro)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("NU_SINISTRO");

                entity.Property(e => e.IdPessoa)
                .HasColumnType("int")
                .HasColumnName("ID_PESSOA");

                entity.Property(e => e.DsNaturezaPessoa)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("DS_NATUREZA_PESSOA");

                entity.Property(e => e.InVitima)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IN_VITIMA");

                entity.Property(e => e.InBeneficiario)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IN_BENEFICIARIO");

                entity.Property(e => e.InProcurador)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IN_PROCURADOR");

                entity.Property(e => e.InRepresentanteLegal)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IN_REPRESENTANTE_LEGAL");

                entity.Property(e => e.InCessionario)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IN_CESSIONARIO");

                entity.Property(e => e.InIntermediario)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IN_INTERMEDIARIO");

                entity.Property(e => e.InPessoaExcluida)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IN_PESSOA_EXCLUIDA");

                entity.Property(e => e.InPessoaExcluida)
                   .HasMaxLength(1)
                   .IsUnicode(false)
                   .HasColumnName("IN_PESSOA_EXCLUIDA");

                entity.Property(e => e.DtExclusaoPessoa)
                    .HasColumnType("datetime")
                    .HasColumnName("DT_EXCLUSAO_PESSOA");

                entity.Property(e => e.NuCnpjPessoa)
                    .HasColumnType("bigint")
                    .HasColumnName("NU_CNPJ_PESSOA");

                entity.Property(e => e.DsTipoCpfVitima)
                   .HasMaxLength(15)
                   .IsUnicode(false)
                   .HasColumnName("DS_TIPO_CPF_VITIMA");

                entity.Property(e => e.NuCpfPessoa)
                   .HasColumnType("bigint")
                   .HasColumnName("NU_CPF_PESSOA");

                entity.Property(e => e.NmPessoa)
                   .HasMaxLength(15)
                   .IsUnicode(false)
                   .HasColumnName("NM_PESSOA");

                entity.Property(e => e.DtNascimentoPessoa)
                    .HasColumnType("datetime")
                    .HasColumnName("DT_NASCIMENTO_PESSOA");

                entity.Property(e => e.NmLogradouroPessoa)
                   .HasMaxLength(600)
                   .IsUnicode(false)
                   .HasColumnName("NM_LOGRADOURO_END_PESSOA");

                entity.Property(e => e.NuLogradouroEndPessoa)
                   .HasMaxLength(30)
                   .IsUnicode(false)
                   .HasColumnName("NU_LOGRADOURO_END_PESSOA");

                entity.Property(e => e.DsComplEnderecoPessoa)
                   .HasMaxLength(600)
                   .IsUnicode(false)
                   .HasColumnName("DS_COMPL_ENDERECO_PESSOA");

                entity.Property(e => e.NmBairroEndPessoa)
                   .HasMaxLength(600)
                   .IsUnicode(false)
                   .HasColumnName("NM_BAIRRO_END_PESSOA");

                entity.Property(e => e.NmMunicipioEndPessoa)
                  .HasMaxLength(600)
                  .IsUnicode(false)
                  .HasColumnName("NM_MUNICIPIO_END_PESSOA");

                entity.Property(e => e.SgUfEndPessoa)
                  .HasMaxLength(60)
                  .IsUnicode(false)
                  .HasColumnName("SG_UF_END_PESSOA");

                entity.Property(e => e.NmUfEndPessoa)
                  .HasMaxLength(60)
                  .IsUnicode(false)
                  .HasColumnName("NM_UF_END_PESSOA");

                entity.Property(e => e.CdCepEndPessoa)
                  .HasMaxLength(30)
                  .IsUnicode(false)
                  .HasColumnName("CD_CEP_END_PESSOA");

                entity.Property(e => e.NuDddTelefonePessoa)
                  .HasMaxLength(30)
                  .IsUnicode(false)
                  .HasColumnName("NU_DDD_TELEFONE_PESSOA");

                entity.Property(e => e.NuTelefonePessoa)
                  .HasMaxLength(30)
                  .IsUnicode(false)
                  .HasColumnName("NU_TELEFONE_PESSOA");

                entity.Property(e => e.NuDddCelularPessoa)
                  .HasMaxLength(30)
                  .IsUnicode(false)
                  .HasColumnName("NU_DDD_CELULAR_PESSOA");

                entity.Property(e => e.NuCelularPessoa)
                  .HasMaxLength(30)
                  .IsUnicode(false)
                  .HasColumnName("NU_CELULAR_PESSOA");

                entity.Property(e => e.DsEmailPessoa)
                  .HasMaxLength(200)
                  .IsUnicode(false)
                  .HasColumnName("DS_EMAIL_PESSOA");

                entity.Property(e => e.DsProfissaoInformadaPessoa)
                  .HasMaxLength(100)
                  .IsUnicode(false)
                  .HasColumnName("DS_PROFISSAO_INFORMADA_PESSOA");

                entity.Property(e => e.DsProfissaoComprovadaPessoa)
                  .HasMaxLength(60)
                  .IsUnicode(false)
                  .HasColumnName("DS_PROFISSAO_COMPROVADA_PESSOA");

                entity.Property(e => e.DsRendaInformadaPessoa)
                  .HasMaxLength(30)
                  .IsUnicode(false)
                  .HasColumnName("DS_RENDA_INFORMADA_PESSOA");

                entity.Property(e => e.DsRendaComprovadaPessoa)
                 .HasMaxLength(100)
                 .IsUnicode(false)
                 .HasColumnName("DS_RENDA_COMPROVADA_PESSOA");

                entity.Property(e => e.NmPessoaTitularCpf)
                 .HasMaxLength(100)
                 .IsUnicode(false)
                 .HasColumnName("NM_PESSOA_TITULAR_CPF");

                entity.Property(e => e.NmMaeTitularCpf)
                 .HasMaxLength(100)
                 .IsUnicode(false)
                 .HasColumnName("NM_MAE_TITULAR_CPF");

                entity.Property(e => e.DtNascimentoTitularCpf)
                   .HasColumnType("datetime")
                   .HasColumnName("DT_NASCIMENTO_TITULAR_CPF");

                entity.Property(e => e.DtObitoTitularCpf)
                   .HasColumnType("datetime")
                   .HasColumnName("DT_OBITO_TITULAR_CPF");

                entity.Property(e => e.DsSexoTitularCpf)
                 .HasMaxLength(50)
                 .IsUnicode(false)
                 .HasColumnName("DS_SEXO_TITULAR_CPF");

                entity.Property(e => e.NmLogradouroEndTitularCpf)
                 .HasMaxLength(100)
                 .IsUnicode(false)
                 .HasColumnName("NM_LOGRADOURO_END_TITULAR_CPF");

                entity.Property(e => e.NuLogradouroEndTitularCpf)
                 .HasMaxLength(50)
                 .IsUnicode(false)
                 .HasColumnName("NU_LOGRADOURO_END_TITULAR_CPF");

                entity.Property(e => e.DsComplEndTitularCpf)
                 .HasMaxLength(60)
                 .IsUnicode(false)
                 .HasColumnName("DS_COMPL_END_TITULAR_CPF");

                entity.Property(e => e.NmBairroEndTitularCpf)
                 .HasMaxLength(60)
                 .IsUnicode(false)
                 .HasColumnName("NM_BAIRRO_END_TITULAR_CPF");

                entity.Property(e => e.NmMunicipioEndTitularCpf)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("NM_MUNICIPIO_END_TITULAR_CPF");

                entity.Property(e => e.SgUfEndTitularCpf)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SG_UF_END_TITULAR_CPF");

                entity.Property(e => e.NmUfEndTitularCpf)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("NM_UF_END_TITULAR_CPF");

                entity.Property(e => e.CdCepEndTitularCpf)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CD_CEP_END_TITULAR_CPF");

                entity.Property(e => e.NuDddTelefoneTitularCpf)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("NU_DDD_TELEFONE_TITULAR_CPF");

                entity.Property(e => e.NuTelefoneTitularCpf)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("NU_TELEFONE_TITULAR_CPF");

                entity.Property(e => e.DsSituacaoTitularCpf)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("DS_SITUACAO_TITULAR_CPF");

                entity.Property(e => e.InCandidatoBeneficiario)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("IN_CANDIDATO_BENEFICIARIO");

                entity.Property(e => e.DsTipoBeneficiario)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("DS_TIPO_BENEFICIARIO");

                entity.Property(e => e.IdPessoaRepresentanteLegal)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("ID_PESSOA_REPRESENTANTE_LEGAL");

                entity.Property(e => e.NmPessoaRepresentanteLegal)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("NM_PESSOA_REPRESENTANTE_LEGAL");

                entity.Property(e => e.DtNascimentoRepLegal)
                   .HasColumnType("datetime")
                   .HasColumnName("DT_NASCIMENTO_REP_LEGAL");

                entity.Property(e => e.NuCpfPessoaRepLegal)
                   .HasColumnType("bigint")
                   .HasColumnName("NU_CPF_PESSOA_REP_LEGAL");

                entity.Property(e => e.DtProcuracao)
                   .HasColumnType("datetime")
                   .HasColumnName("DT_PROCURACAO");

                entity.Property(e => e.SgUfProcuracao)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("SG_UF_PROCURACAO");

                entity.Property(e => e.NmUfProcuracao)
               .HasMaxLength(60)
               .IsUnicode(false)
               .HasColumnName("NM_UF_PROCURACAO");

                entity.Property(e => e.VlEmitidoPessoa)
                .HasColumnType("float")
               .HasColumnName("VL_EMITIDO_PESSOA");

                entity.Property(e => e.VlPagoPessoa
                )
                .HasColumnType("float")
               .HasColumnName("VL_PAGO_PESSOA");

                entity.Property(e => e.VlAPagarPessoa
                )
                .HasColumnType("float")
               .HasColumnName("VL_A_PAGAR_PESSOA");

                entity.Property(e => e.VlJurosMultaPago
                )
                .HasColumnType("float")
               .HasColumnName("VL_JUROS_MULTA_PAGO");

                entity.Property(e => e.DtUltimoPagamento)
               .HasMaxLength(30)
               .IsUnicode(false)
               .HasColumnName("DT_ULTIMO_PAGAMENTO");

                entity.Property(e => e.CdBancoPessoa)
               .HasMaxLength(30)
               .IsUnicode(false)
               .HasColumnName("CD_BANCO_PESSOA");

                entity.Property(e => e.NmBancoPessoa)
               .HasMaxLength(100)
               .IsUnicode(false)
               .HasColumnName("NM_BANCO_PESSOA");

                entity.Property(e => e.NuAgenciaBancariaPessoa)
               .HasMaxLength(30)
               .IsUnicode(false)
               .HasColumnName("NU_AGENCIA_BANCARIA_PESSOA");

                entity.Property(e => e.NuDvAgenciaBancariaPessoa)
               .HasMaxLength(3)
               .IsUnicode(false)
               .HasColumnName("NU_DV_AGENCIA_BANCARIA_PESSOA");

                entity.Property(e => e.DsTipoContaBancariaPessoa)
               .HasMaxLength(20)
               .IsUnicode(false)
               .HasColumnName("DS_TIPO_CONTA_BANCARIA_PESSOA");

                entity.Property(e => e.NuContaBancariaPessoa)
               .HasMaxLength(15)
               .IsUnicode(false)
               .HasColumnName("NU_CONTA_BANCARIA_PESSOA");

                entity.Property(e => e.NuDvContaBancariaPessoa)
               .HasMaxLength(30)
               .IsUnicode(false)
               .HasColumnName("NU_DV_CONTA_BANCARIA_PESSOA");

                entity.Property(e => e.NuDvaContaBancariaPessoa)
               .HasMaxLength(30)
               .IsUnicode(false)
               .HasColumnName("NU_DVA_CONTA_BANCARIA_PESSOA");

                entity.Property(e => e.IdPessoaUnificada)
              .HasMaxLength(30)
              .IsUnicode(false)
              .HasColumnName("ID_PESSOA_UNIFICADA");

                entity.Property(e => e.DtInclusaoDados)
              .HasMaxLength(30)
              .IsUnicode(false)
              .HasColumnName("DT_INCLUSAO_DADO");

            });

           
        }
    }
}
