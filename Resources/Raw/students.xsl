<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="yes"/>

	<xsl:template match="/Students">
		<html>
			<head>
				<title>Успішність студентів</title>
				<style>
					body { font-family: Arial, sans-serif; padding: 20px; }
					table { border-collapse: collapse; width: 100%; margin-bottom: 20px; }
					th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
					th { background-color: #4CAF50; color: white; }
					.student-row { background-color: #f2f2f2; font-weight: bold; }
				</style>
			</head>
			<body>
				<h2>Звіт про успішність студентів</h2>
				<table>
					<tr>
						<th>Студент</th>
						<th>Факультет / Кафедра</th>
						<th>Курс / Група</th>
						<th>Дисципліна</th>
						<th>Оцінка</th>
						<th>Статус</th>
					</tr>
					<xsl:for-each select="Student">
						<xsl:for-each select="Disciplines/Discipline">
							<tr>
								<xsl:if test="position() = 1">
									<td rowspan="{count(../../Disciplines/Discipline)}">
										<xsl:value-of select="../../@FullName"/>
									</td>
									<td rowspan="{count(../../Disciplines/Discipline)}">
										<xsl:value-of select="../../@Faculty"/>
										<xsl:if test="../../@Department">
											/ <xsl:value-of select="../../@Department"/>
										</xsl:if>
									</td>
									<td rowspan="{count(../../Disciplines/Discipline)}">
										Курс: <xsl:value-of select="../../@Course"/>, <xsl:value-of select="../../@Group"/>
									</td>
								</xsl:if>
								<td>
									<xsl:value-of select="@Name"/>
								</td>
								<td>
									<xsl:value-of select="@Grade"/>
								</td>
								<td>
									<xsl:value-of select="@Status"/>
								</td>
							</tr>
						</xsl:for-each>
					</xsl:for-each>
				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>