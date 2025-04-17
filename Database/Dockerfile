FROM postgres
COPY init.sql /docker-entrypoint-initdb.d/
COPY postgresql.conf /etc/postgresql.conf
RUN mkdir /var/arch
CMD ["-c", "config_file=/etc/postgresql.conf"]
